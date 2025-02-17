
using System.IO;
using UnityEditor;
using UnityEngine;

namespace D.Editor
{
    public static class BuildPacketEditor
    {
        /// <summary>
        /// 构建 Windows 资源包
        /// </summary>
        [MenuItem("Assets/Tool/Build/Windows")]
        private static void CreateSceneALL_Windows()
        {
            Caching.ClearCache();
            if (!Directory.Exists(Const.ABPath))
            {
                Directory.CreateDirectory(Const.ABPath);
            }
            BuildPipeline.BuildAssetBundles(Const.ABPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
            AssetDatabase.Refresh();
            Util.Log(" build windows assetbundle complete! ");
        }

        /// <summary>
        /// 构建 Android 资源包
        /// </summary>
        [MenuItem("Assets/Tool/Build/Android")]
        private static void CreateSceneALL_Android()
        {
            Caching.ClearCache();
            if (!Directory.Exists(Const.ABPath))
            {
                Directory.CreateDirectory(Const.ABPath);
            }
            BuildPipeline.BuildAssetBundles(Const.ABPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
            AssetDatabase.Refresh();
            Util.Log(" build android assetbundle complete! ");
        }

        /// <summary>
        /// 构建 iOS 资源包
        /// </summary>
        [MenuItem("Assets/Tool/Build/iOS")]
        private static void CreateSceneALL_MAC()
        {
            Caching.ClearCache();
            if (!Directory.Exists(Const.ABPath))
            {
                Directory.CreateDirectory(Const.ABPath);
            }
            BuildPipeline.BuildAssetBundles(Const.ABPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
            AssetDatabase.Refresh();
            Util.Log(" build iOS assetbundle complete! ");
        }
    }
}