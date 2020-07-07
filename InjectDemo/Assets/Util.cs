using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    /// <summary>
    ///  计算指定文件的CRC32值
    /// </summary>
    /// <param name="fileName">指定文件的完全限定名称</param>
    /// <returns>返回值的字符串形式</returns>
    public static uint ComputeCRC32(String fileName)
    {
        uint crcValue = 0;
        //检查文件是否存在，如果文件存在则进行计算，否则返回空值
        if (System.IO.File.Exists(fileName))
        {

            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //计算文件的CSC32值
                Common_Base.CRC32 calculator = new Common_Base.CRC32();
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                calculator.update(buffer);
                crcValue = calculator.getValue();

            }//关闭文件流
        }
        return crcValue;
    }//ComputeCRC32
}
