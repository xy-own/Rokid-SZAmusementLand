using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SZ10001
{
    public class SZ10001Manager : MonoBehaviour
    {
        private GameObject m_UIMgr;
        private GameObject m_NPCMgr;
        private GameObject m_Audio;
        private AudioManager m_AudioManager;
        private GameObject m_Enter;
        private GameObject m_Tietle;
        private GameObject m_JianTou;
        private bool m_IsPlaying;
        private bool m_IsPlayed;
        private Animator m_DoorAni;
        
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
            m_Audio = transform.Find("Audio").gameObject;
            m_AudioManager = new AudioManager(m_Audio.GetComponent<AudioSource>());
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            m_Enter.AddComponent<TriEvent>().exitAction += ExitEvent;
            m_Tietle = transform.Find("Tietle").gameObject;
            m_Tietle.transform.SetParent(Camera.main.transform);
            m_Tietle.transform.localPosition = new Vector3(0f, 0.1f, 1.5f);
            m_DoorAni = m_NPCMgr.transform.Find("DaMen/Ani").GetComponent<Animator>();
            m_JianTou = transform.Find("JianTou").gameObject;

            StartCoroutine(StartAni());
        }

        private void EnterEvent()
        {
            if (!m_IsPlaying)
            {
                m_IsPlaying = true;
                //StartCoroutine(StartAni());
            }
        }
        private void ExitEvent()
        {
            if (m_IsPlayed)
            {
                gameObject.SetActive(false);
            }
        }

        IEnumerator StartAni()
        {
            m_Tietle.SetActive(true);
            yield return new WaitForSeconds(2f);
            m_Tietle.SetActive(false);
            m_NPCMgr.transform.Find("DaMen").gameObject.SetActive(true);
            m_AudioManager.AudioPlayOneShot("DaMenChuXian");
            m_AudioManager.AudioPlay("00-0");
            yield return new WaitForSeconds(28.5f);
            m_UIMgr.SetActive(true);
            m_AudioManager.AudioPlay("00-1");
            yield return new WaitForSeconds(10.5f);
            m_AudioManager.AudioPlay("00-2");
            yield return new WaitForSeconds(23.72f);
            m_DoorAni.SetTrigger("OpenDoor");
            yield return new WaitForSeconds(2f);
            m_JianTou.SetActive(true);
            m_IsPlayed = true;
            MessageDispatcher.SendMessageData("ShowAllPoi");
        }
    }
}
