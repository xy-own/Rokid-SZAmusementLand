using System.IO;
using UnityEditor;
using UnityEngine;

namespace D.Editor
{
    public static class PacketEditor
    {
        /// <summary>
        /// 给选中的目录下所有资源设置标签
        /// </summary>
        [MenuItem("Assets/Tool/BuildPackage", false, 0)]
        private static void BuildPackage()
        {
            Object selectedObject = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(selectedObject);

            int index = path.IndexOf(Const.ResPath);
            if (index < 0)
            {
                Util.Log($" not resource directory ,set label failed ", LogType.Error);
                return;
            }
            string fileName = path.Substring(Const.ResPath.Length + 1);
            int fineId = 0;
            if (!int.TryParse(fileName, out fineId))
            {
                Util.Log($" not resource directory ,set label failed ", LogType.Error);
                return;
            }

            Util.Log($" 是资源包  打包  {fileName} ");
            // do something...
        }

        /// <summary>
        /// 给所有目录下的资源设置标签
        /// </summary>
        [MenuItem("Assets/Tool/BuildAllPackage", false, 2)]
        private static void BuildAllPackage()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            DirectoryInfo dirTempInfo = new DirectoryInfo(Const.ResPath);
            DirectoryInfo[] dirScenesDIRArray = dirTempInfo.GetDirectories();

            foreach (DirectoryInfo currentDIR in dirScenesDIRArray)
            {
                FindResInDirectory(currentDIR, currentDIR.Name);
            }
            AssetDatabase.Refresh();
            Util.Log(" build all package label complete! ");
        }
        /// <summary>
        /// 清理所有目录下的资源标签
        /// </summary>
        [MenuItem("Assets/Tool/UnBuildAllPackage", false, 3)]
        private static void UnBuildAllPackage()
        {
            DirectoryInfo dirTempInfo = new DirectoryInfo(Const.ResPath);
            DirectoryInfo[] dirScenesDIRArray = dirTempInfo.GetDirectories();

            foreach (DirectoryInfo currentDIR in dirScenesDIRArray)
            {
                FindResInDirectory(currentDIR, currentDIR.Name, true);
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            Util.Log(" unbuild all package label complete! ");
        }

        #region Find Res
        private static void FindResInDirectory(FileSystemInfo fileSysInfo, string abName, bool clear = false)
        {
            if (!fileSysInfo.Exists)
            {
                Util.Log("文件或者目录名称:" + fileSysInfo + "不存在请检查", LogType.Error);
                return;
            }
            DirectoryInfo dirInfoObj = fileSysInfo as DirectoryInfo;
            FileSystemInfo[] fileSysArray = dirInfoObj.GetFileSystemInfos();

            foreach (FileSystemInfo fileInfo in fileSysArray)
            {
                // 忽略代码文件
                if (fileInfo.Name.Equals("Script"))
                {
                    continue;
                }
                // 忽略meta文件
                if (fileInfo.Extension == ".meta")
                {
                    continue;
                }
                FileInfo fileinfoObj = fileInfo as FileInfo;
                if (fileinfoObj != null)
                {
                    SetLabel(fileinfoObj, abName, clear);
                }
                else
                {
                    FindResInDirectory(fileInfo, abName, clear);
                }
            }
        }
        /// <summary>
        /// 设置资源标签
        /// </summary>
        /// <param name="fileinfoObj"></param>
        /// <param name="abName"></param>
        private static void SetLabel(FileInfo fileinfoObj, string abName, bool clear = false)
        {
            int tmpIndex = fileinfoObj.FullName.IndexOf("Assets");
            string strAssetFilePath = fileinfoObj.FullName.Substring(tmpIndex);
            AssetImporter tmpImporterObj = AssetImporter.GetAtPath(strAssetFilePath);

            string name = abName;
            // string variant = "unity3d";
            if (clear)
            {
                name = string.Empty;
                // variant = string.Empty;
            }
            tmpImporterObj.assetBundleName = name;
            // tmpImporterObj.assetBundleVariant = variant;
        }
        #endregion


        // //目录打包
        // public static bool PackDir(string res, string pattern, SearchOption searchOption)
        // {
        //     string[] files = GetDirFiles(res, pattern, searchOption);
        //     if (files.Length == 0) return false;
        //     string fn = res.Trim('/') + abDirExtens;

        //     foreach (var file in files)
        //     {
        //         AssetImporter impoter = AssetImporter.GetAtPath(file);
        //         impoter.assetBundleName = fn;
        //     }
        //     return true;
        // }
        // //打包目录中所有资源
        // public static bool PackDirDir(string res, string pattern)
        // {
        //     string[] Dirs = Directory.GetDirectories(res);
        //     if (Dirs.Length == 0) return false;
        //     foreach (var d in Dirs)
        //     {
        //         PackDir(d.Replace('\\', '/'), pattern, SearchOption.AllDirectories);
        //     }
        //     return true;
        // }
    }
}