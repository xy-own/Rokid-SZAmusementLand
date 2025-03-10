using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10006
{
    public class UIManager : MonoBehaviour
    {
        private GameObject m_Tip1;
        private GameObject m_Tip2;
        // Start is called before the first frame update
        void Start()
        {
            m_Tip1 = transform.Find("Tip").gameObject;
            m_Tip2 = transform.Find("Tip1").gameObject;
            MessageDispatcher.AddListener("10006ShowUI", ShowTip);
            MessageDispatcher.AddListener("10006HideUI", HideTip);
            MessageDispatcher.AddListener<string>("10006HideBiao", HideBiao);
            MessageDispatcher.AddListener<string>("10006ShowBiao", ShowBiao);
            MessageDispatcher.AddListener("10006ShowUI1",ShowTip1);
            MessageDispatcher.AddListener("10006HideUI1", HideTip1);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ShowTip()
        {
            m_Tip1.SetActive(true);
            //transform.Find("Biao").gameObject.SetActive(true);
        }

        private void HideBiao(string name)
        {
            transform.Find($"Biao/{name}").gameObject.SetActive(false);
        }

        private void ShowBiao(string name)
        {
            transform.Find($"Biao/{name}").gameObject.SetActive(true);
        }

        private void ShowTip1()
        {
            m_Tip2.SetActive(true);
        }


        public void HideTip()
        {
            m_Tip1.SetActive(false);
        }
        public void HideTip1()
        {
            m_Tip2.SetActive(false);
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener("10006ShowUI", ShowTip);
            MessageDispatcher.RemoveListener("10006HideUI", HideTip);
        }
    }
}
