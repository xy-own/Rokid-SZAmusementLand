using D;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture;

namespace SZ10004
{
    public class NPCManager : MonoBehaviour
    {
        private GameObject m_Scene;
        private GameObject m_JingLing;
        private Animator m_JingLingAni;
        private bool isCanPlay;
        private bool isPlayed;
        private bool isFirstPlay = true;
        private GameObject m_Pink;
        private Animator m_HuaYuanAni;
        private GameObject m_ZhuZi;
        private GameObject m_Flower1;
        private GameObject m_Flower2;
        private GameObject m_Flower3;
        private GameObject m_Wood;
        private GameObject m_YinFu1;
        private GameObject m_YinFu2;
        private GameObject m_YinFu3;

        private GameObject m_JianTou;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                GetGrip();
            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                GetScissors();
            }
            if (Input.GetKeyUp(KeyCode.Y))
            {
                GetPalm();
            }
        }

        public void Initialized()
        {
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_Scene = transform.Find("Scene").gameObject;
            m_JingLing = transform.Find("JingLing").gameObject;
            m_JingLingAni = m_JingLing.transform.Find("HuaYuanJingLing_Rig").GetComponent<Animator>();
            m_HuaYuanAni = m_Scene.transform.Find("NPC/Mod/HuaYuan").GetComponent<Animator>();
            m_Flower1 = m_HuaYuanAni.transform.Find("sunflower").gameObject;
            m_Flower2 = m_HuaYuanAni.transform.Find("flower02").gameObject;
            m_Flower3 = m_HuaYuanAni.transform.Find("Petal").gameObject;
            m_Pink = transform.Find("Pink").gameObject;
            m_Wood = m_Scene.transform.Find("NPC/Mod/HuaYuan/wood02").gameObject;
            m_YinFu1 = transform.Find("YinFu/root/Yin03").gameObject;
            m_YinFu2 = transform.Find("YinFu/root/Yin01").gameObject;
            m_YinFu3 = transform.Find("YinFu/root/Yin02").gameObject;
            m_JianTou = transform.Find("JianTou").gameObject;
        }

        public void StartGame()
        {
            m_Scene.SetActive(true);
            StartCoroutine(AppearAndStartGame());
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            m_JingLing.SetActive(false);
            m_Flower1.SetActive(false);
            m_Flower2.SetActive(false);
            m_Flower3.SetActive(false);
            m_YinFu1.SetActive(false);
            m_YinFu2.SetActive(false);
            m_YinFu3.SetActive(false);
            m_JianTou.SetActive(false);
            isCanPlay = false;
            m_ZhuZi.SetActive(true);
            m_JianTou.SetActive(false);
        }

        IEnumerator AppearAndStartGame()
        {
            m_ZhuZi.SetActive(false);
            m_JingLing.SetActive(true);
            MessageDispatcher.SendMessageData("10004AudioShot", "XCJ");
            m_JingLing.transform.DOScale(Vector3.one*0.5f, 2f).OnComplete(() => {
                m_JingLing.GetComponent<FollowItem>().enabled = false;
                m_JingLing.transform.LookAt(Camera.main.transform.position);
            });
            yield return new WaitForSeconds(4f);
            m_JingLing.GetComponent<FollowItem>().enabled = true;
            m_JingLingAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10004AudioPlay","04-1-1");
            yield return new WaitForSeconds(11.2f);
            m_JingLingAni.SetTrigger("Idle");
            yield return new WaitForSeconds(1f);
            m_JingLingAni.SetTrigger("IdleSpeak");
            MessageDispatcher.SendMessageData("10004AudioPlay", "04-1-2");
            yield return new WaitForSeconds(6.6f);
            m_JingLing.GetComponent<FollowItem>().enabled = false;
            m_JingLing.transform.LookAt(new Vector3(0.11f, m_JingLing.transform.position.y, 6.5f));
            m_JingLingAni.SetTrigger("Fly");
            m_JingLing.transform.DOLocalMove(new Vector3(0.64f, 0.53f, 6.8f), 2f).OnComplete(() => {
                m_JingLing.GetComponent<FollowItem>().enabled = true;
                m_JingLingAni.SetTrigger("Speak1");
                MessageDispatcher.SendMessageData("10004AudioPlay", "04-2");
                m_JingLing.transform.LookAt(Camera.main.transform.position);
                m_Wood.SetActive(true);
            });
            yield return new WaitForSeconds(16.5f);
            MessageDispatcher.SendMessageData("10004ShowUI");
            m_JingLingAni.SetTrigger("Idle");
            isCanPlay = true;
        }

        //识别到拳头
        public void GetGrip()
        {
            if (isCanPlay && !m_Flower1.activeSelf)
            {
                if (isFirstPlay)
                {
                    isCanPlay = false;
                }
                MessageDispatcher.SendMessageData("10004AudioShot", "ShengZhang");
                m_Flower1.SetActive(true);
                m_YinFu1.SetActive(true);
                StartCoroutine(FlowerShengZhang("Flower1"));
                //PlayedGame();
            }
        }

        //识别到剪刀
        public void GetScissors()
        {
            if (isCanPlay && !m_Flower2.activeSelf)
            {
                if (isFirstPlay)
                {
                    isCanPlay = false;
                }
                MessageDispatcher.SendMessageData("10004AudioShot", "ShengZhang");
                m_Flower2.SetActive(true);
                m_YinFu2.SetActive(true);
                StartCoroutine(FlowerShengZhang("Flower2"));
                //PlayedGame();
            }
        }

        //识别到手掌
        public void GetPalm()
        {
            if (isCanPlay && !m_Flower3.activeSelf)
            {
                if (isFirstPlay)
                {
                    isCanPlay = false;
                }
                MessageDispatcher.SendMessageData("10004AudioShot", "ShengZhang");
                m_Flower3.SetActive(true);
                m_YinFu3.SetActive(true);
                StartCoroutine(FlowerShengZhang("Flower3"));
                //PlayedGame();
            }
        }

        IEnumerator FlowerShengZhang(string Name)
        {
            MessageDispatcher.SendMessageData("10004HideUI");
            yield return new WaitForSeconds(1.6f);
            MessageDispatcher.SendMessageData("10004AudioPlay", Name);
            MessageDispatcher.SendMessageData("StopBgm");
            yield return new WaitForSeconds(15.5f);
            if (name == "Flower1")
            {
                m_YinFu1.SetActive(false);
            }
            if (name == "Flower2")
            {
                m_YinFu2.SetActive(false);
            }
            if (name == "Flower3")
            {
                m_YinFu3.SetActive(false);
            }
            MessageDispatcher.SendMessageData("PlayBgm");
            PlayedGame();
        }

        public void PlayedGame()
        {
            if (!isPlayed && isFirstPlay)
            {
                isPlayed = true;
                m_JingLingAni.SetTrigger("Speak3");
                MessageDispatcher.SendMessageData("10004AudioPlay", "04-4-1");
                StartCoroutine(ComGame());
            }
        }

        IEnumerator ComGame()
        {
            yield return new WaitForSeconds(7.2f);
            m_JingLingAni.SetTrigger("Magic");
            MessageDispatcher.SendMessageData("10004AudioShot", "mofashuijin");
            yield return new WaitForSeconds(2f);
            m_JingLingAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10004AudioPlay", "04-4-2");
            MessageDispatcher.SendMessageData("10004AudioShot", "ShuiJingChuXian");
            m_Pink.SetActive(true);
            yield return new WaitForSeconds(1f);
            m_Pink.transform.Find("ef_shuijing_pink/RJ_huapu").gameObject.SetActive(true);
            //MessageDispatcher.SendMessageData("10004ShowUI1");
            yield return new WaitForSeconds(5f);
            //m_JingLingAni.SetTrigger("Idle");
            yield return new WaitForSeconds(3.6f);
            m_JingLingAni.SetTrigger("Idle");
            m_Pink.transform.Find("ef_shuijing_pink/RJ_huapu").gameObject.GetComponent<Animator>().SetTrigger("XiaoShi");
            Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            dis = new Vector3(dis.x, dis.y - 3f, dis.z);
            m_Pink.transform.DOMove(dis, 1.5f);
            m_Pink.transform.DOScale(0f, 1.5f);
            yield return new WaitForSeconds(2f);
            m_JingLingAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10004AudioPlay", "04-5");
            yield return new WaitForSeconds(5.8f);
            m_JingLingAni.SetTrigger("Idle2");
            isFirstPlay = false;
            isCanPlay = true;
            MessageDispatcher.SendMessageData<bool>("10004Played", true);
            MessageDispatcher.SendMessageData("10004AudioPlay", "EndTip");
            m_JianTou.SetActive(true);
        }
    }
}

