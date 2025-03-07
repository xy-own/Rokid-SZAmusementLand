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
            m_JingLingAni = m_JingLing.transform.Find("Ani/SenLingJingLing_Rig").GetComponent<Animator>();
            m_Scene = transform.Find("Scene").gameObject;
            m_SuDi = transform.Find("SuDi").gameObject;
            m_SuDiAni = m_SuDi.transform.Find("SuDi_rig").GetComponent<Animator>();
            m_SuNi = transform.Find("SuNi").gameObject;
            m_SuNiAni = m_SuNi.transform.Find("SuNi_rig").GetComponent<Animator>();
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_GiftBox = transform.Find("LiHe").gameObject;

            m_Palm = transform.Find("TengMan").gameObject;
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
            MessageDispatcher.SendMessageData("10002HideUI");
            //m_Palm.SetActive(false);
            m_Palm.transform.DOMove(m_JingLing.transform.position, 2f).OnComplete(() => { 
                m_Palm.SetActive(false);
            });
            StartCoroutine(PalmEvent());
        }

        IEnumerator AppearAndStartGame()
        {
            m_ZhuZi.SetActive(false);
            yield return new WaitForSeconds(1f);
            m_SuDi.SetActive(true);
            m_SuNi.SetActive(true);
            MessageDispatcher.SendMessageData("10002AudioShot", "XCJ");
            m_SuDi.transform.DOScale(Vector3.one, 2f);
            m_SuNi.transform.DOScale(Vector3.one, 2f);
            yield return new WaitForSeconds(4f);
            m_SuDiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-1-1");
            yield return new WaitForSeconds(14.6f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-1");
            yield return new WaitForSeconds(10.4f);
            MessageDispatcher.SendMessageData("10002ShowUI");
            m_SuNiAni.SetTrigger("Idle");
            m_Palm.SetActive(true);
        }

        IEnumerator PalmEvent()
        {
            m_JingLing.SetActive(true);
            yield return new WaitForSeconds(4f);
            m_JingLingAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData<string>("SetBgm", "BGM2");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-1");
            yield return new WaitForSeconds(8.7f);
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-2");
            yield return new WaitForSeconds(6.1f);
            m_JingLingAni.SetTrigger("Wave");
            yield return new WaitForSeconds(3f);
            m_GiftBox.SetActive(true);
            m_GiftBox.transform.DOScale(Vector3.one, 2f);
            yield return new WaitForSeconds(2f);
            m_JingLingAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-3");
            yield return new WaitForSeconds(13f);
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-4");
            yield return new WaitForSeconds(9f);
            m_JingLingAni.SetTrigger("Idle");
            isCanPlay = true;
            MessageDispatcher.SendMessageData("10002ShowUI1");
        }
        
        public void GetPalm()
        {
            if (isCanPlay)
            {
                MessageDispatcher.SendMessageData("10002HideUI1");
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
            m_JingLing.transform.DOLocalJump(new Vector3(5.95f, -1.94f, 6.75f), 1, 1, 1.5f);
            yield return new WaitForSeconds(1.5f);
            m_JingLingAni.SetTrigger("Speak4");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-3-5");
            yield return new WaitForSeconds(14.9f);
            m_JingLingAni.SetTrigger("Idle");
            m_SuNi.GetComponent<FollowItem>().enabled = false;
            m_SuNi.transform.LookAt(new Vector3( m_JingLing.transform.position.x,m_SuNi.transform.position.y,m_JingLing.transform.position.z));
            m_SuNiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-2");
            yield return new WaitForSeconds(5.7f);
            m_SuNi.GetComponent<FollowItem>().enabled = true;
            m_SuNiAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-1-2");
            yield return new WaitForSeconds(11.45f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak4");
            MessageDispatcher.SendMessageData("10002AudioPlay", "01-2-3");
            yield return new WaitForSeconds(3.05f);
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

