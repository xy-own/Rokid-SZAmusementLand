using D;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace SZ10003
{
    public class NPCManager : MonoBehaviour
    {
        private GameObject m_SuNi;
        private GameObject m_SuDi;
        private GameObject m_Scene;
        private GameObject m_MaoMaoChong;
        private Animator m_SuNiAni;
        private Animator m_SuDiAni;
        private Animator m_MaoMaoChongAni;
        private GameObject m_ZhuZi;
        private GameObject m_Box;
        private GameObject m_Effect;
        private GameObject m_JianTou;
        private GameObject m_EndPos;

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
            m_SuDi = transform.Find("SuDi").gameObject;
            m_SuDiAni = m_SuDi.transform.Find("SuDi_rig").GetComponent<Animator>();
            m_SuNi = transform.Find("SuNi").gameObject;
            m_SuNiAni = m_SuNi.transform.Find("SuNi_rig").GetComponent<Animator>();
            m_MaoMaoChong = transform.Find("MaoMaoChong").gameObject;
            m_MaoMaoChongAni = m_MaoMaoChong.transform.Find("MaoMaoChong_Rig").GetComponent<Animator>();
            m_Box = transform.Find("Box").gameObject;
            m_Effect = transform.Find("Effect").gameObject;
            m_JianTou = transform.Find("JianTou").gameObject;
            m_EndPos = transform.Find("SuDiEndPos").gameObject;
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
            m_SuDi.SetActive(true);
            m_SuNi.SetActive(true);
            m_SuDi.transform.LookAt(new Vector3(Camera.main.transform.position.x,m_SuDi.transform.position.y, Camera.main.transform.position.z));
            MessageDispatcher.SendMessageData("10003AudioShot", "XCJ");
            yield return new WaitForSeconds(4f);
            m_SuDiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-1-1");
            yield return new WaitForSeconds(9.6f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Shout");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-2-1");
            yield return new WaitForSeconds(3.9f);
            m_SuNiAni.SetTrigger("Idle");
            yield return new WaitForSeconds(2f);
            m_Effect.SetActive(true);
            MessageDispatcher.SendMessageData("10003AudioShot", "MaoMaoChongChuXian");
            yield return new WaitForSeconds(2f);
            m_MaoMaoChong.SetActive(true);
            m_MaoMaoChong.transform.DOScale(1f, 2f);
            MessageDispatcher.SendMessageData<string>("SetBgm", "BGM3");
            yield return new WaitForSeconds(2f);
            m_MaoMaoChongAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-1");
            m_SuNi.transform.LookAt(m_MaoMaoChong.transform.position);
            m_SuDi.transform.LookAt(m_MaoMaoChong.transform.position);
            yield return new WaitForSeconds(8f);
            m_MaoMaoChongAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-2");
            yield return new WaitForSeconds(4.5f);
            m_SuNiAni.SetTrigger("Idle");
            m_MaoMaoChongAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-3");
            yield return new WaitForSeconds(17.8f);
            m_MaoMaoChongAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-4");
            yield return new WaitForSeconds(5.2f);
            m_SuDiAni.SetTrigger("Idle");
            m_MaoMaoChongAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-5");
            yield return new WaitForSeconds(5.9f);
            m_MaoMaoChongAni.SetTrigger("Idle");
            m_MaoMaoChong.GetComponent<FollowItem>().enabled = true;
            m_Box.SetActive(true);
            m_Box.transform.position = Camera.main.transform.position;
            yield return new WaitForSeconds(1f);
            Vector3 dis = m_MaoMaoChong.transform.position + m_MaoMaoChong.transform.forward;
            dis = new Vector3(dis.x, dis.y + 1f, dis.z);
            MessageDispatcher.SendMessageData("10003AudioShot", "Move");
            m_Box.transform.DOMove(dis, 2f).OnComplete(() => {
                m_MaoMaoChongAni.SetTrigger("Speak3");
                MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-6");
            });
            yield return new WaitForSeconds(10f);
            m_MaoMaoChongAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-7");
            yield return new WaitForSeconds(2.9f);
            m_SuNiAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-8");
            yield return new WaitForSeconds(1.8f);
            m_SuDiAni.SetTrigger("Idle");
            m_Box.SetActive(false);
            MessageDispatcher.SendMessageData("10003AudioShot", "MaoMaoChongLiKai");
            m_MaoMaoChongAni.SetTrigger("Bullet");
            m_MaoMaoChong.transform.DOLocalMove(new Vector3(-1.08f, 2.93f, -1.16f), 3f).OnComplete(() =>
            {
                m_Effect.SetActive(false);
                m_SuNi.transform.LookAt(m_SuDi.transform.position);
                m_SuDi.transform.LookAt(m_SuNi.transform.position);
                m_SuNiAni.SetTrigger("Anxious");
                MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-9");
            });
            yield return new WaitForSeconds(6.3f);
            m_SuNiAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Anxiou");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-10");
            yield return new WaitForSeconds(10f);
            m_SuDiAni.SetTrigger("Idle");
            //m_SuNi.transform.LookAt(new Vector3(Camera.main.transform.position.x, m_SuDi.transform.position.y, Camera.main.transform.position.z));
            //m_SuDi.transform.LookAt(new Vector3(Camera.main.transform.position.x, m_SuDi.transform.position.y, Camera.main.transform.position.z));
            m_SuNi.GetComponent<FollowItem>().enabled = true;
            m_SuDi.GetComponent<FollowItem>().enabled = true;
            m_SuNiAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-11");
            yield return new WaitForSeconds(4.8f);
            m_SuNiAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-12");
            yield return new WaitForSeconds(14.6f);
            m_SuDiAni.SetTrigger("Run");
            m_SuDi.GetComponent<FollowItem>().enabled = false;
            m_SuDi.transform.LookAt(new Vector3(m_EndPos.transform.position.x, m_SuDi.transform.position.y, m_EndPos.transform.position.z));
            m_SuDi.transform.DOLocalMove(m_EndPos.transform.localPosition, 2f).OnComplete(() => {
                m_SuDi.SetActive(false);
            });
            m_SuNiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10003AudioPlay", "02-3-13");
            yield return new WaitForSeconds(1.5f);
            m_SuNiAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10003Played", true);
            MessageDispatcher.SendMessageData("10003AudioPlay", "EndTip");
            m_JianTou.SetActive(true);
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            m_SuDi.SetActive(false);
            m_SuNi.SetActive(false);
            m_JianTou.SetActive(false);
        }
    }
}

