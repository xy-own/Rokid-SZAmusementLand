using UnityEngine;

/// <summary>
/// Interface Manager Object
/// </summary>
public class ManagementCenter {
    /// <summary>
    /// 游戏管理器对象
    /// </summary>
    private static GameObject _managerObject = null;
    public static GameObject managerObject {
        get {
            if (_managerObject == null)
                _managerObject = GameObject.FindWithTag("GameManager");
            return _managerObject;
        }
    }
    private static Main _main = null;
    public static Main main
    {
        get {
            if (_main == null)
            {
                _main = managerObject.GetComponent<Main>();
            }
            return _main;
        }
    }

    public static T GetManager<T>() where T : class
    {
        if (typeof(T) == typeof(ConfigManager))
        {
            return Main.configMgr as T;
        }
        return Main.GetManager<T>();
    }
}