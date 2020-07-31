using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static string fileName = "Assembly-CSharp.patch.bytes";

    public static string tmp1 = "tmp1";

    public static Dictionary<string, string> configDic = new Dictionary<string, string>();

    public static List<string> configList = new List<string>();

    public static void DoSth()
    {
        Debug.Log("do sth");
    }
}
