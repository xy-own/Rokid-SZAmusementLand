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
            m_Tietle = transform.Find("Tietle").gameObject;
            m_Tietle.transform.SetParent(Camera.main.transform);
        }

        private void EnterEvent()
        {
            StartCoroutine(StartAni());
        }

        IEnumerator StartAni()
        {
            m_Tietle.SetActive(false);
            m_AudioManager.AudioPlay("00-0");
            yield return new WaitForSeconds(18f);
            m_UIMgr.SetActive(true);
            m_AudioManager.AudioPlay("00-1");
            yield return new WaitForSeconds(8.7f);
            m_AudioManager.AudioPlay("00-2");
        }
    }
}
