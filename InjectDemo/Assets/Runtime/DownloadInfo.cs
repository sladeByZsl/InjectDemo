using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yunchang.Download
{
    public class DownloadInfo
    {
        public string url { get; set; }
        public string path { get; set; }

        public long totalSize { get; set; }
        public long currentSize { get; set; }

        public override string ToString()
        {
            return string.Format("url:{0},path:{1},totalSize:{2},currentSize:{3}",url,path,totalSize,currentSize);
        }
    }
}
