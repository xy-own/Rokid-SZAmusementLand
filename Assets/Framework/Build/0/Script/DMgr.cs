using UnityEngine;
using D.Base;
using SZ10001;
using Unity.VisualScripting;
using DG.Tweening;
using System.Collections.Generic;

namespace D_0
{
    public class DMgr : MonoBase
    {
        private AudioSource m_Audio;
        private AudioManager m_AudioManager;
        private GameObject m_Road;

        private GameObject m_Poi;

        public List<GameObject> m_List;
        public override void OnEnter(string data)
        {

        }

        public override void OnExit()
        {
        }

        private void Start()
        {
            m_Audio = transform.Find("MainAudio").GetComponent<AudioSource>();
            m_AudioManager = new AudioManager(m_Audio);

            m_Poi = transform.Find("Poi").gameObject;

            m_Road = transform.Find("Poi/RoadList").gameObject;
            AudioPlay("BGM0");
            MessageDispatcher.AddListener<string>("SetBgm",AudioPlay);
            MessageDispatcher.AddListener("StopBgm", AudioStop);
            MessageDispatcher.AddListener("PlayBgm", AudioPlay);

            MessageDispatcher.AddListener("EnterPoi", EnterPoi);
            MessageDispatcher.AddListener("ExitPoi", ExitPoi);

            MessageDispatcher.AddListener("ShowAllPoi", ShowAllPoi);
        }

        private void AudioPlay(string Name)
        {
            m_AudioManager.AudioPlay(Name,true);
        }

        private void AudioStop() 
        {
            m_AudioManager.AudioStop();
        }

        private void AudioPlay()
        {
            m_AudioManager.AudioPlay();
        }

        private void EnterPoi()
        {
            m_Road.SetActive(false);
        }

        private void ExitPoi()
        {
            m_Road.SetActive(true);
        }

        private void ShowAllPoi()
        {
            for (int i = 1;i < m_List.Count;i++)
            {
                m_List[i].SetActive(true);
            }
        }
    }
}