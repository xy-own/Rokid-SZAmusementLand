using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10004
{
    public class UIManager : MonoBehaviour
    {
        private GameObject m_Tip1;
        private GameObject m_Tip2;
        // Start is called before the first frame update
        void Start()
        {
            m_Tip1 = transform.Find("Tip").gameObject;
            m_Tip2 = transform.Find("Tip2").gameObject;
            MessageDispatcher.AddListener("10004ShowUI", ShowTip);
            MessageDispatcher.AddListener("10004HideUI", HideTip);
            MessageDispatcher.AddListener("10004ShowUI1", ShowTip1);
            MessageDispatcher.AddListener("10004HideUI1", HideTip1);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ShowTip()
        {
            m_Tip1.SetActive(true);
        }

        public void HideTip()
        {
            m_Tip1.SetActive(false);
        }
        private void ShowTip1()
        {
            m_Tip2.SetActive(true);
        }

        public void HideTip1()
        {
            m_Tip2.SetActive(false);
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener("10004ShowUI", ShowTip);
            MessageDispatcher.RemoveListener("10004HideUI", HideTip);
            MessageDispatcher.AddListener("10004ShowUI1", ShowTip1);
            MessageDispatcher.AddListener("10004HideUI1", HideTip1);
        }
    }
}

