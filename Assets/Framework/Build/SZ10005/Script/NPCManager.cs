using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        private bool isCanPlay;
        private bool isPlayed;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialized()
        {
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_Scene = transform.Find("Scene").gameObject;
            m_BoHe = transform.Find("BoHuoSir").gameObject;
            m_BoHeAni = m_BoHe.transform.Find("BoHuoSir_rig").GetComponent<Animator>();
            m_Page = transform.Find("Page").gameObject;
            m_Book = m_Scene.transform.Find("Model/book_Rig").gameObject;
            M_Green = transform.Find("Green").gameObject;
            m_BookAni = m_Book.GetComponent<Animator>();

            MessageDispatcher.AddListener("GetDrawCom", GetDrawCon);
            //MessageDispatcher.AddListener("DrawFail", DrawFail);
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
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-1");
            yield return new WaitForSeconds(5.5f);
            m_BoHeAni.SetTrigger("Welcom");
            yield return new WaitForSeconds(3.8f);
            m_BoHeAni.SetTrigger("Think");
            yield return new WaitForSeconds(13f);
            m_BoHeAni.gameObject.GetComponent<ChangeSpeakAni>().enabled = true;
            m_Page.SetActive(true);
            m_Page.transform.DOScale(0.7f, 1f);
            m_Page.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            m_BoHeAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-2");
            yield return new WaitForSeconds(12.8f);
            m_BoHeAni.SetTrigger("Idle");
            isCanPlay = true;
            MessageDispatcher.SendMessageData("10005ShowUI");
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            isCanPlay = false;
            m_ZhuZi.SetActive(true);
        }

        private void GetDrawCon()
        {
            if (!isPlayed)
            {
                //StopCoroutine(Fail());
                m_BoHeAni.SetTrigger("Idle");
                MessageDispatcher.SendMessageData("10005AudioStop");
                isPlayed = true;


                MessageDispatcher.SendMessageData("10005AudioShot", "mofashuijin");
                MessageDispatcher.SendMessageData("10005HideUI");
                StartCoroutine(ComGame());
            }
        }

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
            m_BoHeAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-3");
            yield return new WaitForSeconds(2f);
            m_Page.transform.DOMove(m_Book.transform.position + m_Book.transform.up, 2f).OnComplete(() =>
            {
                m_Page.SetActive(false);
                m_BookAni.SetTrigger("Open");
            });
            yield return new WaitForSeconds(6f);
            m_BookAni.SetTrigger("Fail");
            M_Green.SetActive(true);
            yield return new WaitForSeconds(2f);
            Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            dis = new Vector3(dis.x, dis.y-3f, dis.z);
            M_Green.transform.DOMove(dis, 1.5f);
            yield return new WaitForSeconds(4.6f);
            m_BoHeAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10005AudioPlay", "03-5");
            yield return new WaitForSeconds(12.6f);
            m_BoHeAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10005Played", true);
        }
    }
}
