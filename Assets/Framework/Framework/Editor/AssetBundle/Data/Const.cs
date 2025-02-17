using System.IO;
using UnityEngine;

namespace D.Editor
{
    public static class Const
    {
        public static string ABPath = Path.Combine(Application.dataPath, "StreamingAssets");

        /// <summary>
        /// Build资源主目录 修改该路径时记得同步修改 LoadMgr.cs 中 AssetDatabase 的加载路径
        /// </summary>
        public static string ResPath = "Assets/VisionTester/Build";
    }
}