using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net;
using System;
using TMPro;

namespace FunPlus.Common.Update
{
    /// <summary>
    /// 通过http下载资源
    /// </summary>
    public class HttpDownLoad
    {
        //下载进度
        float progress;
        public float downloadProgress
        {
            get
            {
                return progress;
            }
        }

        public bool reLoad = true;

        public bool error = false;

        //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
        public bool isStop;
        //子线程负责下载，否则会阻塞主线程，Unity界面会卡主
        private Thread thread;
        //表示下载是否完成
        public bool isDone { get; private set; }

        public void Init()
        {
            progress = 0;
            isDone = false;
            error = false;
        }
        /// <summary>
        /// 下载方法(断点续传)
        /// </summary>
        /// <param name="url">URL下载地址</param>
        /// <param name="savePath">Save path保存路径</param>
        /// <param name="callBack">Call back回调函数</param>
        public void DownLoad(string url, string savePath)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                error = true;
                return;
            }
            //开启子线程下载,使用匿名方法
            thread = new Thread(delegate ()
            {
                Init();
                FileStream fs = null;
                long fileLength = 0;
                long totalLength = 0;
                Stream stream = null;
                try
                {
                    //判断保存路径是否存在
                    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    totalLength = GetLength(url);
                    if (totalLength <= 0)
                    {
                        isDone = true;
                        error = true;
                        return;
                    }
                    //使用流操作文件
                    if (File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                    //获取文件现在的长度
                    fileLength = fs.Length;
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Update thread error--check file---{0}", e.ToString()));
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                    isDone = true;
                    error = true;
                    return;
                }


                try
                {
                    //如果没下载完
                    if (fileLength < totalLength)
                    {
                        fs.Seek(fileLength, SeekOrigin.Begin);

                        HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                        request.Timeout = 6000;
                        request.ReadWriteTimeout = 6000;
                        request.AddRange((int)fileLength);
                        stream = request.GetResponse().GetResponseStream();

                        byte[] buffer = new byte[1024];
                        //使用流读取内容到buffer中
                        //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
                        int length = stream.Read(buffer, 0, buffer.Length);

                        while (length > 0)
                        {
                            if (isStop) break;
                            fs.Write(buffer, 0, length);
                            fs.Flush();
                            //计算进度
                            fileLength += length;
                            progress = (float)fileLength / (float)totalLength;
                            // UnityEngine.Debug.LogError(progress);
                            length = stream.Read(buffer, 0, buffer.Length);
                        }

                        stream.Close();
                        stream.Dispose();

                    }

                    else
                    {
                        progress = 1;
                    }
                    fs.Close();
                    fs.Dispose();

                    if (progress == 1)
                    {
                        isDone = true;
                    }
                    else
                    {
                        isDone = true;
                        error = true;
                    }
                }
                catch (WebException e)
                {
                    Debug.LogError(string.Format("Update thread error---download---{0}", e.Status));
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }

                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }


                    isDone = true;
                    error = true;
                }

            });
            thread.IsBackground = true;

            thread.Start();
        }


        /// <summary>
        /// 获取下载文件的大小
        /// </summary>
        /// <returns>The length.</returns>
        /// <param name="url">URL.</param>
        long GetLength(string url)
        {
            long size = -1;
            try
            {
                HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
                requet.Method = "HEAD";
                if (requet == null)
                {
                    return size;
                }

                HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
                if (response == null)
                {
                    reLoad = false;
                    return size;
                }
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        size = response.ContentLength;// 从文件头得到远程文件的长度;
                        break;
                    case HttpStatusCode.NotModified:
                    case HttpStatusCode.NotFound:
                        reLoad = false;
                        break;
                    default:
                        break;

                }

                return size;
            }
            catch (WebException e)
            {
                string mesg = e.Message;
                if (mesg.IndexOf("500") > 0 || mesg.IndexOf("401") > 0 || mesg.IndexOf("404") > 0)
                    reLoad = false;
                return size;
            }

        }

    }
}

