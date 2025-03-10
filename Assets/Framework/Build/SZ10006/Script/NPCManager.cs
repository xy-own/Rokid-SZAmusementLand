using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture.Button;

namespace SZ10006
{
    enum CristalType
    {
        None,
        Red,
        Blue,
        Yellow
    }

    public class NPCManager : MonoBehaviour
    {
        private GameObject m_LanBoShu;
        private Animator m_LanBoShuAni;
        private GameObject m_RedBox;
        private Animator m_RedBoxAni;
        private GameObject m_BlueBox;
        private Animator m_BlueBoxAni;
        private GameObject m_YellowBox;
        private Animator m_YellowBoxAni;
        private GameObject m_RedCristal;
        private GameObject m_BlueCristal;
        private GameObject m_YellowCristal;
        private GameObject m_Scene;
        private bool isCanPlay;
        private CristalType m_ChooseType = CristalType.None;
        private GameObject m_ChooseCristal;
        private GameObject m_finger;
        private bool m_IsRedCom;
        private bool m_IsBlueCom;
        private bool m_IsYellowCom;
        private GameObject m_Blue;
        private GameObject m_ZhuZi;
        private GameObject m_BlueEffect;
        private GameObject m_RedEffect;
        private GameObject m_YellowEffect;

        private Transform m_RedPos;
        private Transform m_BluePos;
        private Transform m_YellowPos;

        private GameObject m_JianTou;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (m_ChooseCristal!=null)
            {
                m_ChooseCristal.transform.position = m_finger.transform.position;
            }
            if (m_IsRedCom && m_IsBlueCom && m_IsYellowCom)
            {
                m_IsRedCom = false;
                m_IsBlueCom = false;
                m_IsYellowCom = false;
                PlayedGame();
            }

        }

        public void Initialize()
        {
            m_ZhuZi = transform.Find("Mod").gameObject;
            m_Scene = transform.Find("Scene").gameObject;
            m_LanBoShu = transform.Find("LanBoShu").gameObject;
            m_LanBoShuAni = m_LanBoShu.transform.Find("LanBoShu_rig").GetComponent<Animator>();
            m_Blue = transform.Find("Blue").gameObject;

            m_RedPos = m_Scene.transform.Find("RedPos");
            m_BluePos = m_Scene.transform.Find("BluePos");
            m_YellowPos = m_Scene.transform.Find("YellowPos");

            m_RedBox = m_Scene.transform.Find("LanBoShu/Box/box_r_T").gameObject;
            m_RedBox.AddComponent<BtnItem>().enterAction += EnterRedBox;
            m_BlueBox = m_Scene.transform.Find("LanBoShu/Box/box_n_T").gameObject;
            m_BlueBox.AddComponent<BtnItem>().enterAction += EnterBlueBox;
            m_YellowBox = m_Scene.transform.Find("LanBoShu/Box/box_y_T").gameObject;
            m_YellowBox.AddComponent<BtnItem>().enterAction += EnterYellowBox;
            m_BlueEffect = m_BlueBox.transform.Find("BlueBoxEffect").gameObject;
            m_RedEffect = m_RedBox.transform.Find("RedBoxEffect").gameObject;
            m_YellowEffect = m_YellowBox.transform.Find("YellowBoxEffect").gameObject;
            m_RedCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_r").gameObject;
            m_RedCristal.AddComponent<BtnItem>().enterAction += EnterRedCristal;
            m_BlueCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_b").gameObject;
            m_BlueCristal.AddComponent<BtnItem>().enterAction += EnterBlueCristal;
            m_YellowCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_y").gameObject;
            m_YellowCristal.AddComponent<BtnItem>().enterAction += EnterYellowCristal;

            m_RedBoxAni = m_RedBox.transform.Find("Group").GetComponent<Animator>();
            m_BlueBoxAni = m_BlueBox.transform.Find("Group").GetComponent<Animator>();
            m_YellowBoxAni = m_YellowBox.transform.Find("Group").GetComponent<Animator>();

            m_JianTou = transform.Find("JianTou").gameObject;
        }

        public void StartGame()
        {
            m_Scene.SetActive(true);
            StartCoroutine(ApperAndStart());
        }

        IEnumerator ApperAndStart()
        {
            m_ZhuZi.SetActive(false);
            m_LanBoShu.SetActive(true);
            MessageDispatcher.SendMessageData("10006AudioShot", "XCJ");
            m_LanBoShu.transform.DOScale(Vector3.one, 1f);
            yield return new WaitForSeconds(1f);
            m_LanBoShuAni.SetTrigger("Speak1");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-1-1");
            yield return new WaitForSeconds(7.2f);
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-1-2");
            yield return new WaitForSeconds(11.4f);
            m_LanBoShuAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-2");
            m_BlueBoxAni.SetTrigger("On");
            MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
            MessageDispatcher.SendMessageData("10006ShowBiao", "Blue");
            yield return new WaitForSeconds(1f);
            m_RedBoxAni.SetTrigger("On");
            MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
            MessageDispatcher.SendMessageData("10006ShowBiao", "Red");
            yield return new WaitForSeconds(1f);
            m_YellowBoxAni.SetTrigger("On");
            MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
            MessageDispatcher.SendMessageData("10006ShowBiao", "Yellow");
            MessageDispatcher.SendMessageData("10006ShowUI");
            yield return new WaitForSeconds(12.4f);
            isCanPlay = true;
            m_LanBoShuAni.SetTrigger("Idle");
        }

        private void EnterRedBox(FingerEvent fingerEvent,Collider go)
        {
            if (m_ChooseType == CristalType.Red)
            {
                m_ChooseCristal = null;
                m_RedCristal.transform.DOScale(0.5f, 1f);
                m_RedCristal.transform.DOMove(m_RedBox.transform.position, 1f).OnComplete(() =>
                {
                    MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
                    m_RedBoxAni.SetTrigger("Off");
                    m_RedCristal.SetActive(false);
                    m_RedEffect.SetActive(true);
                    m_IsRedCom = true;
                });
                MessageDispatcher.SendMessageData("10006HideBiao", "Red");
            }
            else if(m_ChooseType != CristalType.None && m_ChooseType != CristalType.Red)
            {
                
            }
        }


        private void EnterBlueBox(FingerEvent fingerEvent, Collider go)
        {
            if (m_ChooseType == CristalType.Blue)
            {
                m_ChooseCristal = null;
                m_BlueCristal.transform.DOScale(0.5f, 1f);
                m_BlueCristal.transform.DOMove(m_BlueBox.transform.position, 1f).OnComplete(() =>
                {
                    MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
                    m_BlueBoxAni.SetTrigger("Off");
                    m_BlueCristal.SetActive(false);
                    m_BlueEffect.SetActive(true);
                    m_IsBlueCom = true;
                });
                MessageDispatcher.SendMessageData("10006HideBiao", "Blue");
            }
            else if (m_ChooseType != CristalType.None && m_ChooseType != CristalType.Blue)
            {

            }
        }

        private void EnterYellowBox(FingerEvent fingerEvent, Collider go)
        {
            if (m_ChooseType == CristalType.Yellow)
            {
                m_ChooseCristal = null;
                m_YellowCristal.transform.DOScale(0.5f, 1f);
                m_YellowCristal.transform.DOMove(m_YellowBox.transform.position, 1f).OnComplete(() =>
                {
                    MessageDispatcher.SendMessageData("10006AudioShot", "KaiGuanXIangZi");
                    m_YellowBoxAni.SetTrigger("Off");
                    m_YellowCristal.SetActive(false);
                    m_YellowEffect.SetActive(true);
                    m_IsYellowCom = true;
                });
                MessageDispatcher.SendMessageData("10006HideBiao", "Yellow");
            }
            else if (m_ChooseType != CristalType.None && m_ChooseType != CristalType.Yellow)
            {

            }
        }

        private void EnterRedCristal(FingerEvent fingerEvent, Collider go)
        {
            m_ChooseType = CristalType.Red;
            m_ChooseCristal = m_RedCristal;
            m_finger = fingerEvent.gameObject;
        }

        private void EnterBlueCristal(FingerEvent fingerEvent, Collider go)
        {
            m_ChooseType = CristalType.Blue;
            m_ChooseCristal = m_BlueCristal;
            m_finger = fingerEvent.gameObject;
        }

        private void EnterYellowCristal(FingerEvent fingerEvent, Collider go)
        {
            m_ChooseType = CristalType.Yellow;
            m_ChooseCristal = m_YellowCristal;
            m_finger = fingerEvent.gameObject;
        }

        public void PlayedGame()
        {
            m_LanBoShuAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-3-1");
            MessageDispatcher.SendMessageData("10006HideUI");
           
            StartCoroutine(ComGame());
        }

       IEnumerator ComGame()
        {
            yield return new WaitForSeconds(7f);
            m_LanBoShuAni.SetTrigger("Idle");
            //m_RedBoxAni.SetTrigger("Off");
            //m_RedCristal.SetActive(false);
            //yield return new WaitForSeconds(1f);
            //m_BlueBoxAni.SetTrigger("Off");
            //m_BlueCristal.SetActive(false);
            //yield return new WaitForSeconds(1f);
            //m_YellowBoxAni.SetTrigger("Off");
            //m_YellowCristal.SetActive(false);
            //yield return new WaitForSeconds(1f);
            m_RedBox.transform.DOLocalMoveX(0f,1f);
            m_YellowBox.transform.DOLocalMoveX(0f, 1f);
            yield return new WaitForSeconds(1f);
            m_BlueBox.transform.DOScale(0f, 1f);
            m_YellowBox.transform.DOScale(0f, 1f);
            m_RedBox.transform.DOScale(0f, 1f);
            m_Blue.SetActive(true);
            MessageDispatcher.SendMessageData("10006AudioShot", "LanShuiJingChuXian");
            m_Blue.transform.Find("ef_shuijing_blue/RJ_YongQi").gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            m_LanBoShuAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-3-2");
            yield return new WaitForSeconds(6.8f);
            m_Blue.transform.Find("ef_shuijing_blue/RJ_YongQi").gameObject.GetComponent<Animator>().SetTrigger("XiaoShi");
            Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward*0.5f;
            dis = new Vector3(dis.x, dis.y - 0.3f, dis.z);
            //m_Blue.transform.DOScale(0.2f, 1f).OnComplete(() => {
              
            //});
            m_Blue.transform.DOMove(dis, 1.5f);
            m_Blue.transform.DOScale(0f, 1.5f);
            yield return new WaitForSeconds(4f);
            m_LanBoShuAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-4");
            yield return new WaitForSeconds(5.9f);
            m_LanBoShuAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10006Played", true);
            m_Scene.transform.Find("LanBoShu/SC_LanBoShu").GetComponent<Animator>().SetTrigger("XiaoShi");
            m_Scene.transform.Find("LanBoShu/SC_LanBoShu/desk").transform.DOScale(0f, 1f);
            m_JianTou.SetActive(true);
        }

        public void RecoverGame()
        {
            m_Scene.SetActive(false);
            m_LanBoShu.SetActive(false);
            m_JianTou.SetActive(false);
        }    
    }
}
