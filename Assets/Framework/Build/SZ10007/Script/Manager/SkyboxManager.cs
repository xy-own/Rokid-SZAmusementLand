using UnityEngine;
using System.IO;

namespace SU10007
{
    /// <summary>
    /// 天空球管理工具类：用于加载Res路径下的天空球材质并更换当前场景的Skybox
    /// </summary>
    public class SkyboxManager : MonoBehaviour
    {
        private string skyboxResourcePath = "Skyboxes";
        private string defaultSkyboxName = "Skybox_mt";
        [Tooltip("当前点位天空球材质的名称")]
        public string targetSkyboxName = "Skybox_mt";

        /// <summary>
        /// 单例实例
        /// </summary>
        private static SkyboxManager _instance;
        public static SkyboxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SkyboxManager");
                    _instance = go.AddComponent<SkyboxManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 当前已加载的天空球材质
        /// </summary>
        private Material[] _availableSkyboxes;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllSkyboxes();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 加载资源目录下的所有天空球材质
        /// </summary>
        public void LoadAllSkyboxes()
        {
            _availableSkyboxes = Resources.LoadAll<Material>(skyboxResourcePath);
            if (_availableSkyboxes.Length == 0)
            {
                Debug.LogWarning($"未在Resources/{skyboxResourcePath}路径找到任何天空球材质");
            }
            else
            {
                Debug.Log($"成功加载 {_availableSkyboxes.Length} 个天空球材质");
            }
        }

        /// <summary>
        /// 通过名称设置天空球
        /// </summary>
        /// <param name="skyboxName">天空球材质的名称</param>
        /// <returns>是否成功设置了天空球</returns>
        public bool SetSkybox(string skyboxName)
        {
            if (_availableSkyboxes == null || _availableSkyboxes.Length == 0)
            {
                Debug.LogWarning("天空球材质尚未加载，请先调用LoadAllSkyboxes()");
                return false;
            }

            foreach (Material skybox in _availableSkyboxes)
            {
                if (skybox.name == skyboxName)
                {
                    RenderSettings.skybox = skybox;
                    Debug.Log($"已成功切换天空球: {skyboxName}");
                    return true;
                }
            }

            Debug.LogWarning($"未找到名为 {skyboxName} 的天空球材质");
            return false;
        }

        /// <summary>
        /// 通过索引设置天空球
        /// </summary>
        /// <param name="index">天空球材质的索引</param>
        /// <returns>是否成功设置了天空球</returns>
        public bool SetSkyboxByIndex(int index)
        {
            if (_availableSkyboxes == null || _availableSkyboxes.Length == 0)
            {
                Debug.LogWarning("天空球材质尚未加载，请先调用LoadAllSkyboxes()");
                return false;
            }

            if (index >= 0 && index < _availableSkyboxes.Length)
            {
                RenderSettings.skybox = _availableSkyboxes[index];
                Debug.Log($"已成功切换天空球: {_availableSkyboxes[index].name}");
                return true;
            }

            Debug.LogWarning($"天空球索引 {index} 超出范围，有效范围为 0-{_availableSkyboxes.Length - 1}");
            return false;
        }

        /// <summary>
        /// 设置默认的天空球
        /// </summary>
        public void SetDefaultSkybox()
        {
            if (!string.IsNullOrEmpty(defaultSkyboxName))
            {
                SetSkybox(defaultSkyboxName);
            }
        }

        /// <summary>
        /// 获取所有可用天空球材质的名称
        /// </summary>
        public string[] GetAvailableSkyboxNames()
        {
            if (_availableSkyboxes == null || _availableSkyboxes.Length == 0)
            {
                return new string[0];
            }

            string[] names = new string[_availableSkyboxes.Length];
            for (int i = 0; i < _availableSkyboxes.Length; i++)
            {
                names[i] = _availableSkyboxes[i].name;
            }

            return names;
        }

        /// <summary>
        /// 直接加载指定路径的天空球材质并设置
        /// </summary>
        /// <param name="resourcePath">相对于Resources文件夹的路径，不包含扩展名</param>
        /// <returns>是否成功设置了天空球</returns>
        public bool LoadAndSetSkybox(string resourcePath)
        {
            Material skybox = Resources.Load<Material>(resourcePath);
            if (skybox != null)
            {
                RenderSettings.skybox = skybox;
                Debug.Log($"已成功加载并设置天空球: {resourcePath}");
                return true;
            }

            Debug.LogWarning($"未能加载路径为 {resourcePath} 的天空球材质");
            return false;
        }
    }
}