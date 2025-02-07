using System;
using System.Collections;
using System.Collections.Generic;
using Rokid.UXR.Module;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
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
        public float checkTime = 3f;
        public List<RokidVoiceModule> voiceInfo;

        private void Start()
        {
#if !UNITY_EDITOR
            StartCoroutine(CheckAuthority());
#endif
        }
        IEnumerator CheckAuthority()
        {
            bool bl = false;
            WaitForSeconds checkTime = new WaitForSeconds(this.checkTime);
            while(!bl)
            {
                bl = Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO");
                if (bl)
                    break;
                Permission.RequestUserPermission("android.permission.RECORD_AUDIO");
                yield return checkTime;
            }
            RegisterVoice(voiceInfo);
        }
        public void RegisterVoice(List<RokidVoiceModule> voiceInfo)
        {
            ModuleManager.Instance.RegistModule("com.rokid.voicecommand.VoiceCommandHelper", false);
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
            OfflineVoiceModule.Instance.ClearAllInstruct();
            OfflineVoiceModule.Instance.Commit();
        }
        private void OnDestroy()
        {
            UnRegisterVoice();
        }
    }
}