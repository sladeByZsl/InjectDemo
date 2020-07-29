using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yunchang.Download
{
    /// <summary>
    /// ��������Ĺ����࣬�̳�CustomYieldInstruction��ͨ��Э�̵�keepWaiting������
    /// </summary>
    public class DownloadAsyncOperation : CustomYieldInstruction
    {
        private int m_ParallelCount;//���е�����
        private event Action<DownloadAsyncOperation> m_Completed;
        public float Progress { get; private set; }
        public IDownloadHandler[] downloadHandles { get; private set; }

        /// <summary>
        /// ʵ��CustomYieldInstruction�ӿڵķ�������ֵΪtrue��ʱ��Э�̻�һֱ�ȴ���false��ʱ�򣬻����
        /// </summary>
        public override bool keepWaiting => !IsDone;

        /// <summary>
        /// Э�̽���ʱ��ί��
        /// </summary>
        public event Action<DownloadAsyncOperation> completed
        {
            add
            {
                m_Completed += value;
            }
            remove
            {
                m_Completed -= value;
            }
        }

        public bool IsDone
        {
            get
            {
                Update();

                long current = 0, total = 0;
                bool mIsAllDownload = true;
                if (downloadHandles == null)
                    return true;
                for (int i = 0; i < downloadHandles.Length; i++)
                {
                    current += downloadHandles[i].info.currentSize;
                    total += downloadHandles[i].info.totalSize;
                    if (downloadHandles[i].state == DownloadState.none || downloadHandles[i].state == DownloadState.downloading)//�����������δ��������������أ�˵����������δ���
                    {
                        mIsAllDownload = false;
                    }
                }
                Progress = (float)((double)current / (double)total);
                if (mIsAllDownload && m_Completed != null)
                    m_Completed(this);
                return mIsAllDownload;
            }
        }

        private void Update()
        {
            if (downloadHandles == null)
                return;
            int count = 0;
            for (int i = 0; i < downloadHandles.Length; i++)
            {
                if (downloadHandles[i].state == DownloadState.downloading)
                    count++;
                else if (downloadHandles[i].state == DownloadState.error)
                    downloadHandles[i].state = DownloadState.none;
            }

            if (count >= m_ParallelCount)
                return;

            for (int i = 0; i < downloadHandles.Length; i++)
            {
                if (downloadHandles[i].state == DownloadState.none)
                {
                    downloadHandles[i].Start();
                    break;
                }
            }
        }

        public void Cancel()
        {
            for (int i = 0; i < downloadHandles.Length; i++)
            {
                if (downloadHandles[i].state == DownloadState.downloading)
                    downloadHandles[i].Cancel();
            }
            downloadHandles = null;
        }

        public static DownloadAsyncOperation Start<T>(DownloadInfo[] files, int parallelCount) where T : IDownloadHandler
        {
            var opt = new DownloadAsyncOperation();
            opt.m_ParallelCount = parallelCount;
            opt.downloadHandles = new IDownloadHandler[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                opt.downloadHandles[i] = (IDownloadHandler)Activator.CreateInstance<T>();
                opt.downloadHandles[i].info = files[i];
                opt.downloadHandles[i].state = DownloadState.none;
            }
            return opt;
        }
    }
}
