using UnityEngine;
using D.Base;
using SZ10001;
using Unity.VisualScripting;

namespace D_0
{
    public class DMgr : MonoBase
    {
        private AudioSource m_Audio;
        private AudioManager m_AudioManager;

        private GameObject m_Poi;
        public override void OnEnter(string data)
        {

        }

        public override void OnExit()
        {
        }

        private void Start()
        {
            m_Audio = transform.Find("MainAudio").GetComponent<AudioSource>();
            m_AudioManager = m_Audio.AddComponent<AudioManager>();

            m_Poi = transform.Find("Poi").gameObject;

            MessageDispatcher.AddListener<string>("SetBgm",AudioPlay);
            MessageDispatcher.AddListener("StopBgm", AudioStop);
        }

        private void AudioPlay(string Name)
        {
            m_AudioManager.AudioPlay(Name);
        }

        private void AudioStop() 
        {
            m_AudioManager.AudioStop();
        }
    }
}