using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using SZ10004;
using UnityEngine;

namespace SZ10006
{
    public class SZ10006Manager : MonoBehaviour
    {
        private GameObject m_UIMgr;
        private UIManager m_UIManager;
        private GameObject m_NPCMgr;
        private NPCManager m_NPCManager;
        private GameObject m_Audio;
        private AudioManager m_AudioManager;
        private GameObject m_Enter;
        private GameObject m_Exit;
        private bool m_IsEnter;
        private bool m_IsPlayed;
        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Initialize()
        {
            m_UIMgr = transform.Find("UI").gameObject;
            m_UIManager = m_UIMgr.AddComponent<UIManager>();
            m_NPCMgr = transform.Find("NPC").gameObject;
            m_NPCManager = m_NPCMgr.AddComponent<NPCManager>();
            m_NPCManager.Initialize();

            m_Audio = transform.Find("Audio").gameObject;
            m_AudioManager = new AudioManager(m_Audio.GetComponent<AudioSource>());
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            m_Exit = transform.Find("Trigger/Exit").gameObject;
            m_Exit.AddComponent<TriEvent>().exitAction += ExitEvent;

            MessageDispatcher.AddListener<string>("10006AudioPlay", AudioPlay);
            MessageDispatcher.AddListener<string>("10006AudioShot", AudioPlayOneShot);
            MessageDispatcher.AddListener("10006AudioStop", AudioStop);
            MessageDispatcher.AddListener<bool>("10006Played", SetPlayed);
        }

        private void EnterEvent()
        {
            if (!m_IsEnter)
            {
                GameConst.m_IsEnter10006 = true;
                MessageDispatcher.SendMessageData("EnterPoi", gameObject.name);
                m_IsEnter = true;
                m_NPCManager.StartGame();
                MessageDispatcher.SendMessageData<string>("SetBgm", "BGM7");
            }
        }

        private void ExitEvent()
        {
            if (m_IsPlayed)
            {
                MessageDispatcher.SendMessageData("ExitPoi", gameObject);
                MessageDispatcher.SendMessageData<string>("SetBgm", "BGM0");
                m_NPCManager.RecoverGame();
                m_IsEnter = false;
                m_IsPlayed = false;
            }
        }

        private void SetPlayed(bool isPlayed)
        {
            m_IsPlayed = isPlayed;
        }

        private void AudioPlay(string name)
        {
            m_AudioManager.AudioPlay(name);
        }

        private void AudioStop()
        {
            m_AudioManager.AudioStop();
        }

        private void AudioPlayOneShot(string name)
        {
            m_AudioManager.AudioPlayOneShot(name);
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener<string>("10006AudioPlay", AudioPlay);
            MessageDispatcher.RemoveListener<string>("10006AudioShot", AudioPlayOneShot);
            MessageDispatcher.RemoveListener("10006AudioStop", AudioStop);
            MessageDispatcher.RemoveListener<bool>("10006Played", SetPlayed);
        }
    }
}
