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

        public FileDownloadHandler(DownloadInfo _info)
            : base(new byte[1024 * 1024])//开辟1M的内存
        {
            info = _info;
        }

        public bool OpenOrCreateFile()
        {
            try
            {
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
                CloseStream();
                m_IsCancel = true;
                return false;
            }
            return true;
        }

        protected override float GetProgress()
        {
            return (float)((double)info.currentSize / (double)info.totalSize);
        }

        protected override void ReceiveContentLength(int contentLength)
        {
            base.ReceiveContentLength(contentLength);
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0 || m_IsCancel || stream == null)
                return false;
            try
            {
                stream.Write(data, 0, dataLength);
                info.currentSize = stream.Length;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        protected override void CompleteContent()
        {
            base.CompleteContent();
            CloseStream();
        }

        public void CloseStream()
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        public void Cancel()
        {
            m_IsCancel = true;
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
                var dirname = Path.GetDirectoryName(info.path);
                if (!Directory.Exists(dirname))
                {
                    Directory.CreateDirectory(dirname);
                }

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
                    m_FileDownloadHandler.CloseStream();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Cancel()
        {
            if (m_FileDownloadHandler != null)
            {
                m_FileDownloadHandler.Cancel();
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
            m_FileDownloadHandler.CloseStream();
            m_FileDownloadHandler = null;
            request.Dispose();
        }

        public float progress => (float)((double)info.currentSize / (double)info.totalSize);
    }
}
