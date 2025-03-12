using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10008
{
    public class NPCManager : MonoBehaviour
    {
        private GameObject m_GongZhuPoint;
        private GameObject m_WangZiPoint;
        private GameObject m_ZhuZi;
        private GameObject m_GongZhu;
        private GameObject m_WangZi;
        private Animator m_WangZiAni;
        private Animator m_GongZhuAni;
        private GameObject m_LiHe;
        private Animator m_LiHeAni;
        private GameObject m_SuDi;
        private Animator m_SuDiAni;
        private GameObject m_SuNi;
        private Animator m_SuNiAni;
        private GameObject m_StartPos;
        private GameObject m_GongZhuPos;
        private GameObject m_WangZiPos;
        private GameObject m_LiHePos;
        private GameObject m_Cloud;
        private GameObject m_Blue;
        private GameObject m_Green;
        private GameObject m_Pink;
        private GameObject m_Effect;
        private GameObject m_Scene;
        private GameObject m_Tietle;

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
            m_GongZhu = transform.Find("GongZhu").gameObject;
            m_GongZhuAni = m_GongZhu.transform.Find("Ani/GongZhu_Rig").GetComponent<Animator>();
            m_WangZi = transform.Find("WangZi").gameObject;
            m_WangZiAni = m_WangZi.transform.Find("Ani/WangZi_rig").GetComponent<Animator>();
            m_SuDi = transform.Find("SuDi").gameObject;
            m_SuDiAni = m_SuDi.transform.Find("SuDi_rig").GetComponent<Animator>();
            m_SuNi = transform.Find("SuNi").gameObject;
            m_SuNiAni = m_SuNi.transform.Find("SuNi_rig").GetComponent<Animator>();
            m_StartPos = transform.Find("StartPos").gameObject;
            m_GongZhuPos = transform.Find("GongZhuPos").gameObject;
            m_WangZiPos = transform.Find("WangZiPos").gameObject;
            m_GongZhuPoint = transform.Find("GongZhuPoint").gameObject;
            m_WangZiPoint = transform.Find("WangZiPoint").gameObject;
            m_LiHe = transform.Find("LiHe").gameObject;
            m_LiHeAni = m_LiHe.transform.Find("effect_lihe/root/LiHe_T").GetComponent<Animator>();
            m_LiHePos = transform.Find("LiHePos").gameObject;
            m_Cloud = transform.Find("Cloud").gameObject;
            m_Blue = transform.Find("Blue").gameObject;
            m_Green = transform.Find("Green").gameObject;
            m_Pink = transform.Find("Pink").gameObject;
            m_Effect = m_Blue.transform.Find("root/buffdimian").gameObject;
            m_Scene = transform.Find("Scene").gameObject;
            m_Tietle = transform.Find("Tietle").gameObject;
        }

        public void StartGame()
        {
            StartCoroutine(AppearAndStartGame());
        }

        IEnumerator AppearAndStartGame()
        {
            m_SuDi.SetActive(true);
            m_SuNi.SetActive(true);
            yield return new WaitForSeconds(1f);
            m_LiHe.transform.position = Camera.main.transform.position+ Camera.main.transform.forward*3f;
            m_LiHe.SetActive(true);
            yield return new WaitForSeconds(2f);
            m_LiHe.transform.DOLocalMove(m_LiHePos.transform.localPosition, 2f).SetEase(Ease.Linear);
            MessageDispatcher.SendMessageData("10008AudioShot", "Move");
            yield return new WaitForSeconds(2f);
            m_Cloud.SetActive(true);
            m_Cloud.transform.DOScale(0.4f, 1f);
            m_GongZhuPoint.SetActive(true);
            m_WangZiPoint.SetActive(true);
            m_GongZhuPoint.transform.DOMove(m_GongZhuPoint.transform.position + m_GongZhuPoint.transform.forward, 0.5f);
            m_WangZiPoint.transform.DOMove(m_GongZhuPoint.transform.position + m_GongZhuPoint.transform.forward, 0.5f);
            yield return new WaitForSeconds(2f);
            m_GongZhuPoint.transform.DOLocalJump(m_GongZhuPos.transform.localPosition, 2f, 1, 2f).OnComplete(()=>
            {
                m_GongZhu.SetActive(true);
                m_Cloud.SetActive(false);
                m_GongZhuPoint.SetActive(false);
                MessageDispatcher.SendMessageData("10008AudioShot", "RongJie");
            });
            m_WangZiPoint.transform.DOLocalJump(m_WangZiPos.transform.localPosition, 2f, 1, 2f).OnComplete(() => { 
                m_WangZi.SetActive(true);
                m_WangZiPoint.SetActive(false);
            });
            yield return new WaitForSeconds(6f);
            m_WangZiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-1");
            yield return new WaitForSeconds(10f);
            m_WangZiAni.SetTrigger("Idle");
            m_GongZhuAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-2");
            yield return new WaitForSeconds(13.9f);
            m_GongZhuAni.SetTrigger("Idle");
            m_SuDiAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-3");
            yield return new WaitForSeconds(3.9f);
            m_GongZhuAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-4");
            m_LiHeAni.SetTrigger("Open");
            yield return new WaitForSeconds(2f);
            m_Green.SetActive(true);
            //m_GongZhuAni.SetTrigger("Speak4");
            //MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-5");
            yield return new WaitForSeconds(6.13f);
            m_Green.transform.DOLocalMove(new Vector3(-6.18f, -1.55f, -0.15f), 1f);
            MessageDispatcher.SendMessageData("10008AudioShot", "Move");
            yield return new WaitForSeconds(1f);
            m_Pink.SetActive(true);
            m_GongZhuAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-5");
            yield return new WaitForSeconds(5.7f);
            m_Pink.transform.DOLocalMove(new Vector3(-6.4f, -1.55f, 2.65f), 1f);
            MessageDispatcher.SendMessageData("10008AudioShot", "Move");
            yield return new WaitForSeconds(1f);
            m_Blue.SetActive(true);
            m_GongZhuAni.SetTrigger("Speak5");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-6");
            yield return new WaitForSeconds(4.4f);
            m_GongZhuAni.SetTrigger("Speak5");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-7");
            m_Pink.transform.DOLocalMove(m_Blue.transform.localPosition, 1f);
            m_Green.transform.DOLocalMove(m_Blue.transform.localPosition, 1f);
            MessageDispatcher.SendMessageData("10008AudioShot", "Move");
            yield return new WaitForSeconds(1f);
            m_Effect.SetActive(true);
            yield return new WaitForSeconds(1f);
            m_Pink.SetActive(false);
            m_Green.SetActive(false);
            m_Blue.SetActive(false);
            m_Scene.SetActive(true);
            yield return new WaitForSeconds(7.2f);
            m_GongZhuAni.SetTrigger("Idle");
            m_WangZiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-1-8");
            yield return new WaitForSeconds(10.66f);
            m_WangZiAni.SetTrigger("Idle");
            m_SuDi.GetComponent<FollowItem>().enabled = true;
            m_SuNi.GetComponent<FollowItem>().enabled = true;
            m_SuDiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-2");
            yield return new WaitForSeconds(6.6f);
            m_SuDiAni.SetTrigger("Idle");
            m_SuNiAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10008AudioPlay", "07-3");
            yield return new WaitForSeconds(9.7f);
            m_SuNiAni.SetTrigger("Idle");
            m_Tietle.SetActive(true);
        }

        public void RecoverGame()
        {

        }
    }
}

