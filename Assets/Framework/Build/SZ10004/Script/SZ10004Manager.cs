using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture;

namespace SZ10004
{
    public class SZ10004Manager : MonoBehaviour
    {
        private GameObject m_UIMgr;
        private UIManager m_UIManager;
        private GameObject m_NPCMgr;
        private NPCManager m_NPCManager;
        private GameObject m_Audio;
        private AudioManager m_AudioManager;
        private GameObject m_Enter;
        private GameObject m_Exit;
        private bool m_IsPlayed;
        private bool m_IsEnter;

        private bool m_IsPalm;
        private bool m_IsGrip;
        private bool m_IsScissors;

        private bool m_IsPlaying;

        private float m_PalmTime;
        private float m_GripTime;
        private float m_ScissorsTime;
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
            m_NPCMgr = transform.GetChild(0).gameObject;
            m_NPCManager = m_NPCMgr.AddComponent<NPCManager>();
            m_NPCManager.Initialized();

            m_Audio = transform.Find("Audio").gameObject;
            m_AudioManager = new AudioManager(m_Audio.GetComponent<AudioSource>());
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            m_Exit = transform.Find("Trigger/Exit").gameObject;
            m_Exit.AddComponent<TriEvent>().exitAction += ExitEvent;

            MessageDispatcher.AddListener<string>("10004AudioPlay", AudioPlay);
            MessageDispatcher.AddListener<string>("10004AudioShot", AudioPlayOneShot);
            MessageDispatcher.AddListener("10004AudioStop", AudioStop);
            MessageDispatcher.AddListener<bool>("10004Played", SetPlayed);


            MessageDispatcher.AddListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent1, PalmEvent);
            MessageDispatcher.AddListener<HandBackEvent>(XY.UXR.API.OpenAPI.RKHandGripEvent, GripEvent);
            MessageDispatcher.AddListener<ScissorsEvent>(XY.UXR.API.OpenAPI.ScissorsEvent, ScissorsEvent);
        }

        private void EnterEvent()
        {
            if (!m_IsEnter)
            {
                MessageDispatcher.SendMessageData("EnterPoi");
                m_IsEnter = true;
                m_NPCManager.StartGame();
                MessageDispatcher.SendMessageData<string>("SetBgm", "BGM6");
            }
            
        }

        private void SetPlayed(bool isPlayed)
        {
            m_IsPlayed = isPlayed;
            m_IsPlaying = true;
        }

        private void ExitEvent()
        {
            if (m_IsPlayed)
            {
                MessageDispatcher.SendMessageData("ExitPoi");
                MessageDispatcher.SendMessageData<string>("SetBgm", "BGM0");
                m_NPCManager.RecoverGame();
                m_IsEnter = false;
                m_IsPlayed = false;
            }
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
                Debug.Log("识别到手掌");
                //m_IsPalm = true;
                m_NPCManager.GetPalm();
            }
            //else
            //{
            //    //m_IsPalm = false;
            //}
        }

        private void GripEvent(HandBackEvent gripEvent)
        {
            if (gripEvent.status)
            {
                Debug.Log("识别到拳头");
                //m_IsGrip = true;
                m_NPCManager.GetGrip();
            }
            //else
            //{
            //    //m_IsGrip = false;
            //}

        }

        private void ScissorsEvent(ScissorsEvent scissorsEvent)
        {
            //if (palmEvent.status)
            //{
                Debug.Log("识别到剪刀");
            m_NPCManager.GetScissors();
                //m_IsScissors = true;
            //}
            //else
            //{
            //    //m_IsScissors = false;
            //}

        }

        //private void PalmTime()
        //{
        //    if (!m_IsPlaying && m_IsPalm)
        //    {
        //        m_PalmTime += Time.deltaTime;
        //        if(m_PalmTime >= 3)
        //        {
        //            m_NPCManager.GetPalm();
        //            m_PalmTime = 0;
        //            m_IsPlaying = true;
        //        }
        //    }
        //    else if (!m_IsPalm)
        //    {
        //        m_PalmTime = 0;
        //    }
        //}

        //private void GripTime()
        //{
        //    if (!m_IsPlaying && m_IsGrip)
        //    {
        //        m_GripTime += Time.deltaTime;
        //        if (m_GripTime >= 3)
        //        {
        //            m_NPCManager.GetGrip();
        //            m_GripTime = 0;
        //            m_IsPlaying = true;
        //        }
        //    }
        //    else if (!m_IsGrip)
        //    {
        //        m_GripTime = 0;
        //    }
        //}

        //private void ScissorsTime()
        //{
        //    if (!m_IsPlaying && m_IsScissors)
        //    {
        //        m_ScissorsTime += Time.deltaTime;
        //        if (m_ScissorsTime >= 3)
        //        {
        //            m_NPCManager.GetScissors();
        //            m_ScissorsTime = 0;
        //            m_IsPlaying = true;
        //        }
        //    }
        //    else if (!m_IsScissors)
        //    {
        //        m_ScissorsTime = 0;
        //    }
        //}
    }
}
