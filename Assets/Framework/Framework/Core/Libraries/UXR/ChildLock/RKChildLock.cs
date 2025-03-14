using D.Define;
using D.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UXR_ChildLock
{
    public class RKChildLock : MonoBehaviour
    {
        private void Awake()
        {
            InitNative();
            AppLock();
        }
        private void OnDestroy()
        {
            AppUnLock();
        }

        private AndroidJavaObject devices = null;
        private void InitNative()
        {
            try
            {
                AndroidJavaObject unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                devices = new AndroidJavaObject("com.unity3d.player.ToStationPro");
                devices?.Call("setContext", new object[] { unityActivity });
            }
            catch (Exception error)
            {
                Log(error.Message, LogColor.Red);
            }
        }

        public void AppLock()
        {
            if (devices == null)
                return;
            string packName = Application.identifier;
            devices?.Call("appLock", new object[] { packName });
        }
        public void AppUnLock()
        {
            if (devices == null)
                return;
            devices?.Call("appUnLock");
        }
        private void Log(string msg, LogColor color)
        {
            Util.Log(msg, color);
        }
    }
}