using System;
using System.Collections;
using System.Collections.Generic;
using Rokid.UXR.Module;
using UnityEngine;
using UnityEngine.Android;
using XY.UXR.API;

namespace XY.UXR.Voice
{
    [Serializable]
    public class RokidVoiceModule
    {
        public LANGUAGE type = LANGUAGE.CHINESE;
        public string chinese;
        public string pinyin;
        public string english;
    }
    public class RKVoiceManager : MonoBehaviour
    {
        public static RKVoiceManager instance;
        [Header("麦克风权限检测间隔")]
        public float checkTime = 3f;
        [Header("配置全局语音识别信息\nUXR暂时不支持中英文混用\n中文配置chinese和pinyin\n英文配置english字段即可")]
        public List<RokidVoiceModule> voiceInfo;

        private void Start()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RKVoiceManager>();
                if (instance == null)
                {
                    GameObject singletonObj = new GameObject(typeof(RKVoiceManager).ToString());
                    instance = singletonObj.AddComponent<RKVoiceManager>();
                    DontDestroyOnLoad(singletonObj);
                }
            }
            // 确保 instance 只有一个
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // 设置不销毁
            DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR
            StartCoroutine(CheckAuthority());
#endif
        }
        IEnumerator CheckAuthority()
        {
            bool bl = false;
            WaitForSeconds checkTime = new WaitForSeconds(this.checkTime);
            while (!bl)
            {
                bl = Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO");
                if (bl)
                    break;
                Permission.RequestUserPermission("android.permission.RECORD_AUDIO");
                yield return checkTime;
            }

            ModuleManager.Instance.RegistModule("com.rokid.voicecommand.VoiceCommandHelper", false);
            RegisterVoice(voiceInfo);
        }
        public void RegisterVoice(List<RokidVoiceModule> voiceInfo)
        {
            foreach (RokidVoiceModule info in voiceInfo)
            {
                bool isChinese = info.type == LANGUAGE.CHINESE;
                OfflineVoiceModule.Instance.AddInstruct(info.type, isChinese ? info.chinese : info.english, isChinese ? info.pinyin : null, gameObject.name, "OnReceive");
            }
            OfflineVoiceModule.Instance.Commit();
        }
        /// <summary>
        /// 语音指令回调
        /// </summary>
        /// <param name="msg"></param>
        private void OnReceive(string msg)
        {
            MessageDispatcher.SendMessageData(OpenAPI.RKVoice, msg);
        }
        /// <summary>
        /// 注销所有语音指令
        /// </summary>
        public void UnRegisterVoice()
        {
            OfflineVoiceModule.Instance?.ClearAllInstruct();
            OfflineVoiceModule.Instance?.Commit();
        }
        private void OnDestroy()
        {
            UnRegisterVoice();
        }
    }
}