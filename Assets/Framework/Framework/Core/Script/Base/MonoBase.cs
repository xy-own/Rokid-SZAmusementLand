using UnityEngine;
using D.Utility;

namespace D.Base
{
    public abstract class MonoBase : MonoBehaviour
    {
        public abstract void OnEnter(string data);
        public abstract void OnExit();


        #region 动态添加组件
        public T GetManager<T>() where T : Component
        {
            // 尝试获取组件，如果不存在则添加并返回
            if (!gameObject.TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        #endregion

        #region Log
        public void Log(string log, int level = 0)
        {
            Util.Log(log, (D.Define.LogColor)level);
        }
        #endregion
    }
}