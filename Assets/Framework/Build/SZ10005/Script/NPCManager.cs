using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XY.UXR.Gesture;
using XY.UXR.Gesture.Button;

namespace SZ10005
{
    public class NPCManager : MonoBehaviour
    {
        private GameObject m_BoHe;
        private Animator m_BoHeAni;
        private GameObject m_Scene;
        private GameObject m_Page;
        private GameObject m_ZhuZi;
        private GameObject M_Green;
        private GameObject m_Book;
        private Animator m_BookAni;
        private Animator m_BookRootAni;
        private bool isCanPlay;
        private bool isPlayed;
        private GameObject m_JianTou;
        private GameObject m_ClickSphere;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                ClickEvent(new HandBackEvent
                {
                    status = true
                });
            }
        }

        public void Initialized()
        {
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_Scene = transform.Find("Scene").gameObject;
            m_BoHe = transform.Find("BoHuoSir").gameObject;
            m_BoHeAni = m_BoHe.transform.Find("BoHuoSir_rig").GetComponent<Animator>();
            m_Page = transform.Find("Page").gameObject;
            m_Book = m_Scene.transform.Find("Model/Book").gameObject;
            M_Green = transform.Find("Green").gameObject;
            m_BookAni = m_Book.transform.Find("root").GetComponent<Animator>();
            m_BookRootAni = m_Book.transform.Find("root/book_Rig").GetComponent<Animator>();
            m_JianTou = transform.Find("JianTou").gameObject;
            //m_ClickSphere = m_Page.transform.Find("Sphere").gameObject;
            //m_ClickSphere.GetComponent<BtnItem>().enterAction += ClickEvent;

            MessageDispatcher.AddListener<HandBackEvent>(XY.UXR.API.OpenAPI.RKHandEvent, ClickEvent);
            //MessageDispatcher.AddListener("DrawFail", DrawFail);
        }

        private void ClickEvent(HandBackEvent handBackEvent)
        {
            if (isCanPlay)
            {
                if (handBackEvent != null && handBackEvent.status)
                {
                    isCanPlay = false;
                    m_Page.GetComponent<Animator>().SetTrigger("Draw");
                    StartCoroutine(ComGame());
                }
            }
        }

        public void StartGame()
        {
            m_Scene.SetActive(true);
            StartCoroutine(AppearAndStartGame());
        }

        IEnumerator AppearAndStartGame()
        {
            m_ZhuZi.SetActive(false);
            m_Scene.SetActive(true);
            m_BoHe.SetActive(true);
            MessageDispatcher.SendMessageData("10005AudioShot", "XCJ");
            m_BoHe.transform.DOScale(Vector3.one, 2f);
            yield return new WaitForSeconds(4f);
            m_BoHeAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-1-1");
            yield return new WaitForSeconds(5f);
            m_BoHeAni.SetTrigger("Welcom");
            yield return new WaitForSeconds(5f);
            m_BoHeAni.SetTrigger("Think");
            yield return new WaitForSeconds(4.4f);
            m_BoHeAni.SetTrigger("Idle");
            yield return new WaitForSeconds(2f);
            MessageDispatcher.SendMessageData("10005AudioShot", "MoFaChuXian");
            m_Book.SetActive(true);
            m_BoHeAni.SetTrigger("Speak1");
            m_BoHeAni.gameObject.GetComponent<ChangeSpeakAni>().enabled = true;
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-1-2");
            yield return new WaitForSeconds(9.9f);
            m_Page.SetActive(true);
            MessageDispatcher.SendMessageData("10005ShowUI");
            m_Page.transform.DOScale(0.1f, 1f);
            m_Page.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            m_BoHeAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-2");
            yield return new WaitForSeconds(15f);
            m_BoHeAni.SetTrigger("Idle");
            isCanPlay = true;
            //m_Page.GetComponent<BoxCollider>().enabled = true;
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            m_BoHe.SetActive(false);
            isCanPlay = false;
            m_ZhuZi.SetActive(true);
            m_JianTou.SetActive(false);
        }

        //private void GetDrawCon()
        //{
        //    if (!isPlayed)
        //    {
        //        //StopCoroutine(Fail());
        //        m_BoHeAni.SetTrigger("Idle");
        //        MessageDispatcher.SendMessageData("10005AudioStop");
        //        isPlayed = true;


        //        MessageDispatcher.SendMessageData("10005AudioShot", "mofashuijin");
        //        MessageDispatcher.SendMessageData("10005HideUI");
        //        StartCoroutine(ComGame());
        //    }
        //}

        //private void DrawFail()
        //{
        //    m_BoHeAni.SetTrigger("Speak1");
        //    MessageDispatcher.SendMessageData("10005AudioPlay", "03-4");
        //    StartCoroutine(Fail());
        //}

        //IEnumerator Fail()
        //{
        //    yield return new WaitForSeconds(11f);
        //    m_BoHeAni.SetTrigger("Idle");
        //}

        IEnumerator ComGame()
        {
            yield return new WaitForSeconds(5f);
            m_BoHeAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData("10005AudioStop");
            isPlayed = true;
            MessageDispatcher.SendMessageData("10005AudioShot", "mofashuijin");
            MessageDispatcher.SendMessageData("10005HideUI");
            yield return new WaitForSeconds(1f);
            m_BoHeAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-3");
            yield return new WaitForSeconds(2f);
            m_BookRootAni.SetTrigger("Open");
            m_Page.transform.DOMove(m_Book.transform.position + m_Book.transform.up, 2f).OnComplete(() =>
            {
                m_Page.SetActive(false);
                m_BookAni.SetTrigger("FuWen");
            });
            yield return new WaitForSeconds(15.9f);
            m_BoHeAni.SetTrigger("Idle");
            m_BookRootAni.SetTrigger("Fall");
            yield return new WaitForSeconds(2f);
            MessageDispatcher.SendMessageData("10005AudioShot", "ShuiJingChuXian");
            M_Green.SetActive(true);
            yield return new WaitForSeconds(1f);
            M_Green.transform.Find("ef_shuijing_green/RJ_Bohe").gameObject.SetActive(true);
            //MessageDispatcher.SendMessageData("10005ShowUI1");
            m_BoHeAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-5");
            yield return new WaitForSeconds(8.9f);
            M_Green.transform.Find("ef_shuijing_green/RJ_Bohe").gameObject.GetComponent<Animator>().SetTrigger("XiaoShi");
            Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            dis = new Vector3(dis.x, dis.y-3f, dis.z);
            M_Green.transform.DOMove(dis, 1.5f);
            M_Green.transform.DOScale(0f, 1.5f);
            yield return new WaitForSeconds(6.5f);
            m_BoHeAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10005Played", true);
            MessageDispatcher.SendMessageData("10005AudioPlay", "EndTip");
            m_JianTou.SetActive(true);
        }
    }
}
