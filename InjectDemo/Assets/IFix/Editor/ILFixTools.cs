using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace IFix.Editor
{
    public class MeshModify :  UnityEditor.Editor
    {
        [MenuItem("Assets/CopyPatch", priority = 38)]
        public static void CopyPatch()
        {
            
            string patchPath = Application.dataPath + "/../Assembly-CSharp.patch.bytes";
            string resPatch = Application.dataPath + "/Resources/Assembly-CSharp.patch.bytes";
            if (File.Exists(patchPath))
            {
                FileUtil.CopyFileOrDirectory(patchPath, resPatch);
            }

            patchPath = Application.dataPath + "/../Assembly-CSharp-firstpass.patch.bytes";
            resPatch = Application.dataPath + "/Resources/Assembly-CSharp-firstpass.patch.bytes";
            if (File.Exists(patchPath))
            {
                FileUtil.CopyFileOrDirectory(patchPath, resPatch);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/ClearPatch", priority = 37)]
        public static void ClearPatch()
        {
            string resPatch = Application.dataPath + "/Resources/Assembly-CSharp.patch.bytes";
            if (File.Exists(resPatch))
            {
                File.Delete(resPatch);
            }

            resPatch = Application.dataPath + "/Resources/Assembly-CSharp-firstpass.patch.bytes";
            if (File.Exists(resPatch))
            {
                File.Delete(resPatch);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/CopyPatchToFTP", priority = 36)]
        public static void CopyPatchToFTP()
        {

            string patchPath = Application.dataPath + "/../Assembly-CSharp.patch.bytes";
            string resPatch ="D:/Share/Assembly-CSharp.patch.bytes";
            if (File.Exists(patchPath))
            {
                if (File.Exists(resPatch))
                {
                    FileUtil.ReplaceFile(patchPath, resPatch);
                }
                else
                {
                    FileUtil.CopyFileOrDirectory(patchPath, resPatch);
                }
               
                Debug.Log("copy "+ patchPath+"  to: "+ resPatch+"  finish");
            }

            patchPath = Application.dataPath + "/../Assembly-CSharp-firstpass.patch.bytes";
            resPatch = "D:/Share/Assembly-CSharp-firstpass.patch.bytes";
            if (File.Exists(patchPath))
            {
                if (File.Exists(resPatch))
                {
                    FileUtil.ReplaceFile(patchPath, resPatch);
                }
                else
                {
                    FileUtil.CopyFileOrDirectory(patchPath, resPatch);
                }
                Debug.Log("copy " + patchPath + "  to: " + resPatch + "  finish");
            }

            Debug.Log("finish");
        }
    }
}
