using System;
using UnityEngine;

namespace XY.UXR.Login
{
    public delegate void RKTcMessageDelegate(string info);
    public delegate void RKTcErrorDelegate(string message);
    public class RKTcCallback : AndroidJavaProxy
    {
        private RKTcMessageDelegate _successAction = null;
        private RKTcErrorDelegate _errorAction = null;

        public RKTcCallback(RKTcMessageDelegate successAction, RKTcErrorDelegate errorAction) : base("com.rokid.rktc.unity.IRKTcCallback")
        {
            _successAction = successAction;
            _errorAction = errorAction;
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        public void success(string token)
        {
            Util.Log($"--------->>> token  {token}");
            Rokid.UXR.Utility.Loom.QueueOnMainThread(() =>
            {
                _successAction?.Invoke(token);
            });
        }

        /// <summary>
        /// 连接断开
        /// </summary>
        public void error(string errorMsg)
        {
            Util.Log($"--------->>> errorMsg {errorMsg}");
            Rokid.UXR.Utility.Loom.QueueOnMainThread(() =>
            {
                _errorAction?.Invoke(errorMsg);
            });
        }
    }
}