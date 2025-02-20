using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10008
{
    public class SZ10008Manager : MonoBehaviour
    {
        private GameObject m_UIMgr;
        private GameObject m_NPCMgr;
        private GameObject m_Audio;
        private AudioManager m_AudioManager;
        private GameObject m_Enter;
        private GameObject m_Exit;
        private GameObject m_Scene;
        private bool m_IsEnter;
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
            m_NPCMgr = transform.Find("NPC").gameObject;
            m_Scene = m_NPCMgr.transform.Find("Scene").gameObject;
            m_Audio = transform.Find("Audio").gameObject;
            m_AudioManager = new AudioManager(m_Audio.GetComponent<AudioSource>());
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            m_Exit = transform.Find("Trigger/Exit").gameObject;
            m_Exit.AddComponent<TriEvent>().exitAction += ExitEvent;
        }

        private void EnterEvent()
        {
            m_Scene.SetActive(true);
            m_IsEnter = true;
        }

        private void ExitEvent()
        {
            if (m_IsEnter)
            {
                m_Scene.SetActive(false);
            }
        }
    }
}
