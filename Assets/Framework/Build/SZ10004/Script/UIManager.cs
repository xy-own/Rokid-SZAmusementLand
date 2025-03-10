using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10004
{
    public class UIManager : MonoBehaviour
    {
        private GameObject m_Tip1;
        // Start is called before the first frame update
        void Start()
        {
            m_Tip1 = transform.Find("Tip").gameObject;
            MessageDispatcher.AddListener("10004ShowUI", ShowTip);
            MessageDispatcher.AddListener("10004HideUI", HideTip);
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

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener("10004ShowUI", ShowTip);
            MessageDispatcher.RemoveListener("10004HideUI", HideTip);
        }
    }
}

