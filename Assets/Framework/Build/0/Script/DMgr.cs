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
        private GameObject m_Light;
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


            m_Light = transform.Find("Light").gameObject;
            AudioPlay("BGM0");
            MessageDispatcher.AddListener<string>("SetBgm",AudioPlay);
            MessageDispatcher.AddListener("StopBgm", AudioStop);
            MessageDispatcher.AddListener("PlayBgm", AudioPlay);

            MessageDispatcher.AddListener<string>("EnterPoi", EnterPoi);
            MessageDispatcher.AddListener<GameObject>("ExitPoi", ExitPoi);

            MessageDispatcher.AddListener("ShowAllPoi", ShowAllPoi);
            MessageDispatcher.AddListener("ShowRoad", ShowRoad);
           
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

        private void EnterPoi(string name)
        {
            if (name != "10007")
            {
                m_Light.SetActive(true);
            }
            else
            {
                m_Light.SetActive(true);
            }
            m_Road.SetActive(false);
            CloseSomePoi(name);
        }

        private void ShowRoad()
        {
            m_Road.SetActive(true);
        }

        private void ExitPoi(GameObject go)
        {
            if (go.name != "10007")
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>($"Prefab/{go.name}"));
                obj.transform.SetParent(go.transform.parent, false);
                obj.name = go.name;
                for (int i = 0; i < m_List.Count; i++)
                {
                    if (m_List[i].name == go.name)
                    {
                        m_List.RemoveAt(i);
                        break;
                    }
                }

                Destroy(go);
                m_List.Add(obj);
            }
            m_Road.SetActive(true);
            ShowAllPoi();
        }

        private void ShowAllPoi()
        {
            for (int i = 1;i < m_List.Count;i++)
            {
                m_List[i].SetActive(true);
            }
        }

        private void CloseSomePoi(string name)
        {
            for (int i = 1; i < m_List.Count; i++)
            {
                if (m_List[i].name != name)
                {
                    m_List[i].SetActive(false);
                }
            }
        }
    }
}