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

            patchPath = Application.dataPath + "/../Assembly-CSharp-firstpass.patch";
            resPatch = Application.dataPath + "/Resources/Assembly-CSharp-firstpass.patch";
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

            resPatch = Application.dataPath + "/Resources/Assembly-CSharp-firstpass.patch";
            if (File.Exists(resPatch))
            {
                File.Delete(resPatch);
            }
            AssetDatabase.Refresh();
        }
    }
}
