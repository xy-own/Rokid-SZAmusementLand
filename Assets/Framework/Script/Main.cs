using UnityEngine;
using System.Collections.Generic;
public class Main : MonoBehaviour
{
    static Dictionary<string, BaseManager> Managers = new Dictionary<string, BaseManager>();
    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    private void Awake()
    {
        AppConst.AppState = AppState.IsPlaying;
        this.Initialize();
    }

    /// <summary>
    /// 每一帧更新
    /// </summary>
    private void Update()
    {
        ///驱动所有的管理器
        foreach (var mgr in Managers)
        {
            if (mgr.Value != null && mgr.Value.isOnUpdate)
            {
                mgr.Value.OnUpdate(Time.deltaTime);
            }
        }
    }
    /// <summary>
    /// 析构函数
    /// </summary>
    private void OnDestroy()
    {
        if (gameMgr != null)
        {
            gameMgr.OnDispose();
        }
        Debug.Log("~Main was destroyed");
    }
    private void OnApplicationQuit()
    {
        AppConst.AppState = AppState.Exiting;
    }

    //---------------------------------------------------

    /// <summary>
    /// 初始化
    /// </summary>
    void Initialize()
    {
        InitSettings();
        InitManager();
        //DontDestroyOnLoad(gameObject);
        if (gameMgr != null)
        {
            gameMgr.Initialize();
        }
    }
    /// <summary>
    /// 初始化设置
    /// </summary>
    static void InitSettings()
    {
        var settings = Util.GetGameSettings();
        if (settings != null)
        {
            AppConst.LogMode = settings.logMode;
            AppConst.GameFrameRate = settings.GameFrameRate;
            AppConst.ShowFps = settings.showFps;
            AppConst.platformType = settings.PlatformType;
        }
        // Rokid.UXR.Interaction.GesEventInput.Instance.Interactor.Find("LeftHandInteractors").gameObject.SetActive(true);
        // Rokid.UXR.Interaction.GesEventInput.Instance.Interactor.Find("RightHandInteractors").gameObject.SetActive(true);
    }

    /// <summary>
    /// 初始化管理器
    /// </summary>
    static void InitManager()
    {
        Util.Log("BaseBehaviour >>> InitManager");

        AddManager<GManager>();
        AddManager<UIManager>();
        AddManager<SoundManager>();
        AddManager<EffectManager>();
        AddManager<SDKManager>();
        AddManager<LogicManager>();
    }
    static T AddManager<T>() where T : BaseManager, new()
    {
        var type = typeof(T);
        var obj = new T();
        Managers.Add(type.Name, obj);
        return obj;
    }

    public static T GetManager<T>() where T : class
    {
        var type = typeof(T);
        if (!Managers.ContainsKey(type.Name))
        {
            return null;
        }
        return Managers[type.Name] as T;
    }

    #region Object
    private static Camera _gameCamera;
    public static Camera gameCamera
    {
        get
        {
            if (_gameCamera == null)
            {
                _gameCamera = Camera.main;
            }
            return _gameCamera;
        }
    }

    private static Canvas _uiCanvas;
    public static Canvas uiCanvas
    {
        get
        {
            if (_uiCanvas == null)
            {
                _uiCanvas = ManagementCenter.main.transform.Find("UICanvas").GetComponent<Canvas>();
            }
            return _uiCanvas;
        }
    }
    #endregion

    #region 管理器

    private static ConfigManager _configMgr;
    public static ConfigManager configMgr
    {
        get
        {
            if (_configMgr == null)
            {
                _configMgr = ConfigManager.Create();
            }
            return _configMgr;
        }
    }
    private static GManager _gameMgr;
    public static GManager gameMgr
    {
        get
        {
            if (_gameMgr == null)
            {
                _gameMgr = GetManager<GManager>();
            }
            return _gameMgr;
        }
    }
    private static UIManager _uiMgr;
    public static UIManager uiMgr
    {
        get
        {
            if (_uiMgr == null)
            {
                _uiMgr = GetManager<UIManager>();
            }
            return _uiMgr;
        }
    }
    private static SoundManager _soundMgr;
    public static SoundManager soundMgr
    {
        get
        {
            if (_soundMgr == null)
            {
                _soundMgr = GetManager<SoundManager>();
            }
            return _soundMgr;
        }
    }
    private static EffectManager _effectMgr;
    public static EffectManager effectMgr
    {
        get
        {
            if (_effectMgr == null)
            {
                _effectMgr = GetManager<EffectManager>();
            }
            return _effectMgr;
        }
    }
    private static SDKManager _sdkMgr;
    public static SDKManager sdkMgr
    {
        get
        {
            if (_sdkMgr == null)
            {
                _sdkMgr = GetManager<SDKManager>();
            }
            return _sdkMgr;
        }
    }
    private static LogicManager _logicManager;
    public static LogicManager logicManager
    {
        get
        {
            if (_logicManager == null)
            {
                _logicManager = GetManager<LogicManager>();
            }
            return _logicManager;
        }
    }
    #endregion
}