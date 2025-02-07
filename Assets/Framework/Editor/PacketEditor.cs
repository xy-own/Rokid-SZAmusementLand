using UnityEngine;
using UnityEditor;
using System.IO;


public class PacketEditor
{
    [MenuItem("Assets/-1. AssetDatabase打印路径")]
    static void GetAssetPath()
    {
        Caching.ClearCache();
        foreach (Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            Debug.Log(AssetDatabase.GetAssetPath(obj));
        }
        AssetDatabase.Refresh();
    }




    [MenuItem("Assets/0. 生成Texture配置文件")]
    static void CreateTextureConfig()
    {
        Caching.ClearCache();
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        AssetConfig info = new AssetConfig();
        for (int i = 0; i < SelectedAsset.Length; ++i)
        {
            Object obj = SelectedAsset[i];
            if (obj.GetType() == typeof(Texture2D))
            {
                Debug.Log("Create AssetBunldes name :" + obj.name);
                info.name.Add(obj.name);
            }
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
    }
    [MenuItem("Assets/1. 生成GameObject配置文件")]
    static void CreateGameObjectConfig()
    {
        Caching.ClearCache();
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        AssetConfig info = new AssetConfig();
        for (int i = 0; i < SelectedAsset.Length; ++i)
        {
            Object obj = SelectedAsset[i];
            if (obj.GetType() == typeof(GameObject))
            {
                Debug.Log("Create AssetBunldes name :" + obj.name);
                info.name.Add(obj.name);
            }
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
    }
    [MenuItem("Assets/3. 生成AudioClip配置文件")]
    static void CreateAudioConfig()
    {
        Caching.ClearCache();
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        AssetConfig info = new AssetConfig();
        for (int i = 0; i < SelectedAsset.Length; ++i)
        {
            Object obj = SelectedAsset[i];
            if (obj.GetType() == typeof(AudioClip))
            {
                Debug.Log("Create AssetBunldes name :" + obj.name);
                info.name.Add(obj.name);
            }
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
    }

    [MenuItem("Assets/4. 生成Material配置文件")]
    static void CreateMaterialConfig()
    {
        Caching.ClearCache();
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        AssetConfig info = new AssetConfig();
        for (int i = 0; i < SelectedAsset.Length; ++i)
        {
            Object obj = SelectedAsset[i];
            if (obj.GetType() == typeof(Material))
            {
                Debug.Log("Create AssetBunldes name :" + obj.name);
                info.name.Add(obj.name);
            }
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
    }
    [MenuItem("Assets/0. 打包模型Windows")]
    static void CreateSceneALL_Windows()
    {
        Caching.ClearCache();
        string path = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/1. 打包模型Android")]
    static void CreateSceneALL_Android()
    {
        Caching.ClearCache();
        string path = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/2. 打包模型iOS")]
    static void CreateSceneALL_MAC()
    {
        Caching.ClearCache();
        string path = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
        AssetDatabase.Refresh();
    }
}
