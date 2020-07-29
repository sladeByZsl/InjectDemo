using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yunchang.Download
{
    public class WebHelper
    {
        /// <summary>
        ///  获取下载文件的大小
        /// </summary>
        /// <param name="url"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool TryGetLength(string url,out long size)
        {
            size = -1;
            try
            {
                HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
                requet.Method = "HEAD";
                if (requet == null)
                {
                    UnityEngine.Debug.LogError("GetFileSize Fail:request is null");
                    return false;
                }

                HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
                if (response == null)
                {
                    UnityEngine.Debug.LogError("GetFileSize Fail:response is null");
                    return false;
                }
                if (response.StatusCode== HttpStatusCode.OK)
                {
                    size = response.ContentLength;// 从文件头得到远程文件的长度;
                    return true;
                }
                else
                {
                    UnityEngine.Debug.LogError("GetFileSize Fail:response code is:"+ response.StatusCode);
                    return false;
                }
            }
            catch (WebException e)
            {
                UnityEngine.Debug.LogError("GetFileSizeFail msg:" + e.Message);
                return false;
            }
        }
    }
}
