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
        private GameObject m_BlueBox;
        private GameObject m_YellowBox;
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

        private Transform m_RedPos;
        private Transform m_BluePos;
        private Transform m_YellowPos;
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

            m_RedBox = m_Scene.transform.Find("LanBoShu/SC_LanBoShu/box_r").gameObject;
            m_RedBox.AddComponent<BtnItem>().enterAction += EnterRedBox;
            m_BlueBox = m_Scene.transform.Find("LanBoShu/SC_LanBoShu/box_b").gameObject;
            m_BlueBox.AddComponent<BtnItem>().enterAction += EnterBlueBox;
            m_YellowBox = m_Scene.transform.Find("LanBoShu/SC_LanBoShu/box_y").gameObject;
            m_YellowBox.AddComponent<BtnItem>().enterAction += EnterYellowBox;
            m_RedCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_r").gameObject;
            m_RedCristal.AddComponent<BtnItem>().enterAction += EnterRedCristal;
            m_BlueCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_b").gameObject;
            m_BlueCristal.AddComponent<BtnItem>().enterAction += EnterBlueCristal;
            m_YellowCristal = m_Scene.transform.Find("LanBoShu/cristal/cristal02_y").gameObject;
            m_YellowCristal.AddComponent<BtnItem>().enterAction += EnterYellowCristal;
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
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-1");
            yield return new WaitForSeconds(11f);
            m_LanBoShuAni.SetTrigger("Speak2");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-2");
            yield return new WaitForSeconds(8f);
            MessageDispatcher.SendMessageData("10006ShowUI");
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
                    m_IsRedCom = true;
                });
            }
            else if(m_ChooseType != null)
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
                    m_IsBlueCom = true;
                });
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
                    m_IsYellowCom = true;
                });
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
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-3");
            MessageDispatcher.SendMessageData("10006HideUI");
            Vector3 dis = Camera.main.transform.position + Camera.main.transform.forward * 3f;
            dis = new Vector3(dis.x, dis.y - 0.3f, dis.z);
            MessageDispatcher.SendMessageData("10006AudioShot", "mofashuijin");
            m_Blue.SetActive(true);
            m_Blue.transform.DOScale(0.2f, 1f).OnComplete(() => {
                m_Blue.transform.DOMove(dis, 1.5f);
                m_Blue.transform.DOScale(0f, 1.5f);
            });
            StartCoroutine(ComGame());
        }

       IEnumerator ComGame()
        {
            m_LanBoShuAni.SetTrigger("Speak3");
            MessageDispatcher.SendMessageData("10006AudioPlay", "05-4");
            yield return new WaitForSeconds(0.3f);
            m_LanBoShuAni.SetTrigger("Idle");
            MessageDispatcher.SendMessageData<bool>("10006Played", true);
        }

        public void RecoverGame()
        {
            
        }    
    }
}
