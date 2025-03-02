using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture.Button;

namespace SZ10002
{
    public class NPCManager : MonoBehaviour
    {
        private GameObject m_JingLing;
        private Animator m_JingLingAni;
        private GameObject m_Scene;
        private GameObject m_SuDi;
        private Animator m_SuDiAni;
        private GameObject m_SuNi;
        private Animator m_SuNiAni;
        private GameObject m_ZhuZi;
        private GameObject m_Palm;
        private bool isCanPlay;
        private bool isPlayed;

        private GameObject m_GiftBox;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                GetPalm();
            }
        }

        public void Initialized()
        {
            m_JingLing = transform.Find("JingLing").gameObject;
            m_JingLingAni = m_JingLing.transform.Find("SenLingJingLing_Rig").GetComponent<Animator>();
            m_Scene = transform.Find("Scene").gameObject;
            m_SuDi = transform.Find("SuDi").gameObject;
            m_SuDiAni = m_SuDi.transform.Find("WangZi_rig").GetComponent<Animator>();
            m_SuNi = transform.Find("SuNi").gameObject;
            m_SuNiAni = m_SuNi.transform.Find("GongZhu_Rig").GetComponent<Animator>();
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_GiftBox = transform.Find("LiHe").gameObject;

            m_Palm = transform.Find("Palm").gameObject;
            m_Palm.AddComponent<BtnItem>().enterAction += EnterPalm;
        }

        public void StartGame()
        {
            m_Scene.SetActive(true);
            StartCoroutine(AppearAndStartGame());
        }

        private void EnterPalm(FingerEvent fingerEvent,Collider go)
        {
            MessageDispatcher.SendMessageData("10002AudioShot", "shouzhan");
            m_Palm.SetActive(false);
            StartCoroutine(PalmEvent());
        }

        IEnumerator AppearAndStartGame()
        {
            m_ZhuZi.SetActive(false);
            m_SuDi.SetActive(true);
            m_SuNi.SetActive(true);
            MessageDispatcher.SendMessageData("10002AudioShot", "XCJ");
            m_SuDi.transform.DOScale(Vector3.one, 2f);
            m_SuNi.transform.DOScale(Vector3.one, 2f);
            yield return new WaitForSeconds(4f);
            m_SuDiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-1-1");
            yield return new WaitForSeconds(13f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-1");
            yield return new WaitForSeconds(10f);
            m_SuNiAni.SetTrigger("Idle");
            m_Palm.SetActive(true);
        }

        IEnumerator PalmEvent()
        {
            m_JingLing.SetActive(true);
            m_JingLing.transform.DOScale(Vector3.one*0.4f, 1f);
            MessageDispatcher.SendMessageData<string>("SetBgm", "BGM2");
            yield return new WaitForSeconds(4.4f);
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-1");
            yield return new WaitForSeconds(11.9f);
            m_JingLingAni.SetTrigger("Wave");
            m_GiftBox.transform.DOScale(Vector3.one, 2f);
            yield return new WaitForSeconds(3.4f);
            m_JingLingAni.SetTrigger("Speak2");
            yield return new WaitForSeconds(10.7f);
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-2");
            yield return new WaitForSeconds(7f);
            m_JingLingAni.SetTrigger("Idle");
            isCanPlay = true;
        }
        
        public void GetPalm()
        {
            if (isCanPlay)
            {
                isCanPlay = false;
                Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
                dis = new Vector3(dis.x, dis.y - 0.3f, dis.z);
                m_GiftBox.transform.DOMove(dis, 1.5f);
                m_GiftBox.transform.DOScale(0f, 1.5f);
                MessageDispatcher.SendMessageData("10002AudioShot", "jinselizi");
                StartCoroutine(GetBox());
            }
        }

        IEnumerator GetBox()
        {
            yield return new WaitForSeconds(1.5f);
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-3");
            yield return new WaitForSeconds(11.4f);
            m_JingLingAni.SetTrigger("Idle");
            m_SuNi.transform.LookAt(new Vector3( m_JingLing.transform.position.x,m_SuNi.transform.position.y,m_JingLing.transform.position.z));
            m_SuNiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-2");
            yield return new WaitForSeconds(6f);
            m_SuNi.transform.LookAt(Camera.main.transform.position);
            m_SuNiAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-1-2");
            yield return new WaitForSeconds(10f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak4");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-3");
            yield return new WaitForSeconds(3f);
            m_SuNiAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10002Played", true);
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            m_JingLing.SetActive(false);
            m_SuDi.SetActive(false);
            m_SuNi.SetActive(false);
            isCanPlay = false;
            m_ZhuZi.SetActive(true);
        }
    }

}

