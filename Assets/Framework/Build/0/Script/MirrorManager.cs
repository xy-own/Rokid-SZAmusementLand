using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10004
{
    public class MirrorManager : MonoBehaviour
    {
        private GameObject m_Mirror;
        private GameObject m_UI;
        private bool IsCanShow;
        public string m_ID;

        private GameObject m_Trigger;
        // Start is called before the first frame update
        void Start()
        {
            Initialized();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Initialized()
        {
            m_Mirror = transform.Find("JingZi").gameObject;
            m_UI = transform.Find("UI").gameObject;
            m_Trigger = transform.Find("Trigger").gameObject;

            m_Trigger.AddComponent<TriEvent>().enterAction += EnterEvent;
            m_Trigger.GetComponent<TriEvent>().exitAction += ExitEvent;
        }

        private void EnterEvent()
        {
            if (m_ID == "10004")
            {
                IsCanShow = GameConst.m_IsEnter10004;
            }
            if (m_ID == "10005")
            {
                IsCanShow = GameConst.m_IsEnter10005;
            }
            if (m_ID == "10006")
            {
                IsCanShow = GameConst.m_IsEnter10006;
            }

            if (IsCanShow)
            {
                m_Mirror.SetActive(true);
                m_UI.SetActive(true);
            }
        }

        private void ExitEvent()
        {
            if (IsCanShow)
            {
                m_Mirror.SetActive(false);
                m_UI.SetActive(false);
            }
        }
    }
}

