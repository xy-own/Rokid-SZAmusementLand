using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture;

namespace SZ10002
{
    public class SZ10002Manager : MonoBehaviour
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
        private GameObject m_Wall;
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
            m_NPCManager.Initialized();
            m_Audio = transform.Find("Audio").gameObject;
            m_AudioManager = new AudioManager(m_Audio.GetComponent<AudioSource>());

            m_Enter = transform.Find("Trigger/Exit").gameObject;
            m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            //m_Exit = transform.Find("Trigger/Exit").gameObject;
            m_Enter.GetComponent<TriEvent>().exitAction += ExitEvent;

            m_Wall = transform.Find("Wall").gameObject;

            MessageDispatcher.AddListener<string>("10002AudioPlay", AudioPlay);
            MessageDispatcher.AddListener<string>("10002AudioShot", AudioPlayOneShot);
            MessageDispatcher.AddListener("10002AudioStop", AudioStop);

            MessageDispatcher.AddListener<bool>("10002Played", SetPlayed);
            MessageDispatcher.AddListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent1, PalmEvent);

        }

        private void EnterEvent()
        {
            if (!m_IsEnter)
            {
                MessageDispatcher.SendMessageData("EnterPoi",gameObject.name);
                m_NPCManager.StartGame();
                m_IsEnter = true;
                m_Wall.SetActive(true);
            }
        }

        private void ExitEvent()
        {
            //if (m_IsPlayed)
            //{
                MessageDispatcher.SendMessageData<string>("SetBgm", "BGM0");
                m_NPCManager.RecoverGame();
                m_IsEnter = false;
                m_IsPlayed = false;
            m_Wall.SetActive(false);
            MessageDispatcher.SendMessageData("ExitPoi", gameObject);
            //}
        }

        private void SetPlayed(bool isPlayed)
        {
            m_IsPlayed = isPlayed;
            MessageDispatcher.SendMessageData("ShowRoad");
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
        private void PalmEvent(PalmEvent palmEvent)
        {
            if (palmEvent.status)
            {
                m_NPCManager.GetPalm();
            }
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener<string>("10002AudioPlay", AudioPlay);
            MessageDispatcher.RemoveListener<string>("10002AudioShot", AudioPlayOneShot);
            MessageDispatcher.RemoveListener("10002AudioStop", AudioStop);

            MessageDispatcher.RemoveListener<bool>("10002Played", SetPlayed);
            MessageDispatcher.RemoveListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent1, PalmEvent);
        }
    }
}
