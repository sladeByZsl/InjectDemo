using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System;

namespace Yunchang.Download
{
    public class HttpWebRequestHandler : IDownloadHandler
    {
        Task m_Task;
        CancellationTokenSource m_TokenSource;
        byte[] m_BufferBytes;

        public DownloadInfo info { get; set; }

        public DownloadState state { get; set; }

        FileStream stream = null;
        HttpWebRequest request = null;
        WebResponse respone = null;
        Stream responseStream = null;

        public HttpWebRequestHandler()
        {
            m_BufferBytes = new byte[1024 * 1024];
            m_TokenSource = new CancellationTokenSource();
            state = DownloadState.none;
        }

        public void Start()
        {
            state = DownloadState.downloading;
            long fileSize = -1;
            if (WebHelper.TryGetLength(info.url,out fileSize))//Ҫ��ǰ��ȡtotalSize
            {
                info.totalSize = fileSize;
                m_Task = Task.Factory.StartNew(Download, m_TokenSource.Token);
            }
            else
            {
                state = DownloadState.error;
                Cancel();
                Debug.LogError("task fail,getFileSize fail:"+info.ToString());
            }
        }

        public void Cancel()
        {
            if (m_TokenSource!=null)
            {
                m_TokenSource.Cancel();
            }

            if (stream != null)
            {
                stream.Close();
            }

            if (respone != null)
            {
                respone.Close();
                respone.Dispose();
            }

            if (responseStream != null)
            {
                responseStream.Close();
            }
        }

        private void Download()
        {
            try
            {
                OpenOrCreateFile();
               
                request = WebRequest.Create(info.url) as HttpWebRequest;
                request.AddRange((int)info.currentSize);
                request.ServicePoint.ConnectionLimit = int.MaxValue;
                respone = request.GetResponse();
                responseStream = respone.GetResponseStream();
                int nReadSize = 0;
                while (info.currentSize < info.totalSize)
                {
                    //�������п���Ҫ�Ƿ�ֹͻȻ��ͣʱ����紥���������ر������Ӷ���������ȡ��д��ʧ��
                    if (responseStream == null || !responseStream.CanRead)
                    {
                        break;
                    }
                    if (stream == null || !stream.CanWrite)
                    {
                        break;
                    }
                    nReadSize = responseStream.Read(m_BufferBytes, 0, m_BufferBytes.Length);
                    if (nReadSize > 0)
                    {
                        stream.Write(m_BufferBytes, 0, nReadSize);
                        info.currentSize += nReadSize;
                    }
                    Thread.Sleep(10);
                }
                state = DownloadState.done;
                Cancel();
            }
            catch (Exception exception)
            {
                Cancel();
                Debug.LogError("download file fail:" + exception.ToString());
                state = DownloadState.error;
            }
        }

        private bool OpenOrCreateFile()
        {
            try
            {
                var dirname = Path.GetDirectoryName(info.path);
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                }
                stream = new FileInfo(info.path).Open(FileMode.OpenOrCreate, FileAccess.Write);
                info.currentSize = stream.Length;
                if (info.currentSize > 0)
                {
                    stream.Seek(info.currentSize, SeekOrigin.Begin);
                }
            }
            catch(Exception exception)
            {
                Debug.LogError("download file fail:" + exception.ToString());
                return false;
            }
            return true;
        }

        public float progress => (float)((double)info.currentSize / (double)info.totalSize);
    }
}