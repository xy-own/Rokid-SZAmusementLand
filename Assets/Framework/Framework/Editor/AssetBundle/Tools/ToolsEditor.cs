using UnityEngine;
using UnityEditor;
using System.IO;
namespace D.Editor
{
    public static class ToolsEditor
    {
        /// <summary>
        /// 打印选中的文件路径
        /// </summary>
        [MenuItem("Assets/Tool/文件路径")]
        private static void AssetPath()
        {
            Caching.ClearCache();
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                Util.Log(path);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 给选中的资源创建配置文件
        /// </summary>
        [MenuItem("Assets/Tool/生成配置文件")]
        private static void CreateConfig()
        {
            Caching.ClearCache();
            Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            Config info = new Config();
            for (int i = 0; i < SelectedAsset.Length; ++i)
            {
                Object obj = SelectedAsset[i];
                Util.Log(" config add :" + obj.name);
                info.name.Add(obj.name);
            }
            string json = JsonUtility.ToJson(info);
            string[] nn = Selection.assetGUIDs;
            string selectName = AssetDatabase.GUIDToAssetPath(nn[0]);
            int index = selectName.LastIndexOf('/');

            string path = selectName.Substring(0, index) + "/config.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Util.Log(" create config complete! ");
        }
    }
}