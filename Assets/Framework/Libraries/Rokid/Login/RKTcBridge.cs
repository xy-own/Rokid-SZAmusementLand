using System;
using UnityEngine;

namespace XY.UXR.Login
{
    public enum Platform
    {
        Phone,
        Station
    }
    public class RKTcBridge
    {
        /// <summary>
        /// Android native桥接
        /// </summary>
        private static AndroidJavaObject _tcManager = null;
        private static AndroidJavaObject TcManager()
        {
            if (_tcManager == null)
            {
                _tcManager = new AndroidJavaObject("com.rokid.rktc.unity.RKTcManager");
            }
            return _tcManager;
        }
        private static AndroidJavaObject _myContext = null;
        private static AndroidJavaObject MyContent()
        {
            if (_myContext == null)
            {
                _myContext = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _myContext;
        }

        /// <summary>
        /// 初始化ThirdCloudManager
        /// </summary>
        public static void TcInit(Platform platform)
        {
            Util.Log("----> " + (int)platform);
#if !UNITY_EDITOR
            TcManager().Call("init", new object[] { MyContent(), (int)platform });
#endif
        }
        /// <summary>
        /// 获取三方授权Token
        /// </summary>
        /// <param name="successAction"> 获取token成功 返回三方授权token </param>
        /// <param name="errorAction"> 获取token失败 返回错误消息 </param>
        public static void GetToken(RKTcMessageDelegate successAction, RKTcErrorDelegate errorAction)
        {
#if !UNITY_EDITOR
            //TcManager().Call("getThreeCloudToken", new object[] { MyContent(), AppConst.RKTcKey, AppConst.RKTcSecurity, new RKTcCallback(successAction, errorAction) });
#endif
        }
        /// <summary>
        /// 获取三方用户信息
        /// </summary>
        /// <param name="successAction"> 获取用户信息成功 返回用户信息</param>
        /// <param name="errorAction">获取用户信息失败 返回错误消息</param>
        public static void GetUserInfo(RKTcMessageDelegate successAction, RKTcErrorDelegate errorAction)
        {
#if !UNITY_EDITOR
            //TcManager().Call("getUserInfo", new object[] { GameConst.rkTcToken, new RKTcCallback(successAction, errorAction) });
#endif
        }
        /**
         * 解除绑定
         *
         * @param context 上下文（非空）
         */
        public static void TcDestroy()
        {
#if !UNITY_EDITOR
            TcManager().Call("onDestroy", new object[] { MyContent() });
#endif
        }

        /// <summary>
        /// 跳转Rokid登录
        /// </summary>
        public static void JumoActivity()
        {
#if !UNITY_EDITOR
            TcManager().Call("jumpActivity", new object[] { MyContent() });
#endif
        }
    }
}