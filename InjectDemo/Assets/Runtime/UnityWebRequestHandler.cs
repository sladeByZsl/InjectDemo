using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Yunchang.Download
{
    public class FileDownloadHandler : DownloadHandlerScript
    {
        FileStream stream;
        bool m_IsCancel;

        DownloadInfo info;

        /// <summary>
        ///  初始化下载句柄，定义每次下载的数据上限为1M
        /// </summary>
        /// <param name="_info"></param>
        public FileDownloadHandler(DownloadInfo _info)
            : base(new byte[1024 * 1024])//开辟1M的内存
        {
            info = _info;
        }

        public bool OpenOrCreateFile()
        {
            try
            {
                var dirname = Path.GetDirectoryName(info.path);
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                }
                m_IsCancel = false;
                stream = File.Open(info.path, FileMode.OpenOrCreate, FileAccess.Write);
                info.currentSize = stream.Length;
                if (stream.Length > 0)
                {
                    stream.Seek(stream.Length, SeekOrigin.Begin);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("download file fail:" + exception.ToString());
                Cancel();
                return false;
            }
            return true;
        }

        protected override float GetProgress()
        {
            return (float)((double)info.currentSize / (double)info.totalSize);
        }

        /// <summary>
        /// 请求下载时的第一个回调函数，会返回需要接收的文件总长度
        /// </summary>
        /// <param name="contentLength"></param>
        protected override void ReceiveContentLength(int contentLength)
        {
            base.ReceiveContentLength(contentLength);
        }

        /// <summary>
        /// 从网络获取数据时候的回调，每帧调用一次
        /// </summary>
        /// <param name="data">接收到的数据字节流，总长度为构造函数定义的1M，并非所有的数据都是新的</param>
        /// <param name="dataLength">接收到的数据长度，表示data字节流数组中有多少数据是新接收到的，即0-dataLength之间的数据是刚接收到的</param>
        /// <returns>返回true为继续下载，返回false为中断下载</returns>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0 || m_IsCancel || stream == null)
                return false;
            try
            {
                if(stream.CanWrite)
                {
                    stream.Write(data, 0, dataLength);
                    info.currentSize = stream.Length;
                }
            }
            catch (Exception exception)
            {
                Cancel();
                Debug.LogError("task fail,ReceiveData fail:" + exception.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 当接受数据完成时的回调
        /// </summary>
        protected override void CompleteContent()
        {
            base.CompleteContent();
            Cancel();
        }

        public void Cancel()
        {
            m_IsCancel = true;
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }
    }


    public class UnityWebRequestHandler : IDownloadHandler
    {
        FileDownloadHandler m_FileDownloadHandler;

        public DownloadInfo info { get; set; }

        public DownloadState state { get; set; }

        public UnityWebRequestHandler()
        {
            state = DownloadState.none;
        }

        public void Start()
        {
            state = DownloadState.downloading;
            long fileSize = -1;
            if (WebHelper.TryGetLength(info.url, out fileSize))//要提前获取totalSize
            {
                info.totalSize = fileSize;
                Download();
            }
            else
            {
                state = DownloadState.error;
                Cancel();
                Debug.LogError("task fail,getFileSize fail:" + info.ToString());
            }
        }

        private void Download()
        {
            try
            {
                m_FileDownloadHandler = new FileDownloadHandler(info);
                if (m_FileDownloadHandler.OpenOrCreateFile())
                {
                    if (info.currentSize == info.totalSize)
                    {
                        state = DownloadState.done;
                    }
                    else
                    {
                        var request = UnityWebRequest.Get(info.url);
                        request.timeout = 60;
                        request.redirectLimit = 1;
                        request.downloadHandler = m_FileDownloadHandler;
                        //Debug.LogError("bytes=" + info.currentSize + "-" + info.totalSize);
                        if (info.currentSize == 0)
                            request.SetRequestHeader("Range", "bytes=0-");
                        else
                            request.SetRequestHeader("Range", "bytes=" + info.currentSize + "-" + info.totalSize);
                        var operation = request.SendWebRequest();
                        operation.completed += OnOperationCompleted;
                    }
                }
                else
                {
                    state = DownloadState.error;
                    Cancel();
                }
            }
            catch (Exception exception)
            {
                Cancel();
                Debug.LogError("download file fail:" + exception.ToString());
            }
        }

        public void Cancel()
        {
            if (m_FileDownloadHandler != null)
            {
                m_FileDownloadHandler.Cancel();
                m_FileDownloadHandler = null;
            }
        }

        private void OnOperationCompleted(AsyncOperation opt)
        {
            var operation = opt as UnityWebRequestAsyncOperation;
            var request = operation.webRequest;

            if (request.isNetworkError || request.isHttpError)
            {
                state = DownloadState.error;
                Debug.LogError("request fail:" + request.error);
            }
            else
            {
                state = DownloadState.done;
            }
            Cancel();
            if (request != null)
            {
                request.Dispose();
            }
        }

        public float progress => (float)((double)info.currentSize / (double)info.totalSize);
    }
}
