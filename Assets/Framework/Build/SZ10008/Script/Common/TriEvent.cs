using System;
using UnityEngine;
using UnityEngine.Events;

namespace SZ10008
{
    public class TriEvent : MonoBehaviour
    {
        public Action enterAction = null;
        public Action exitAction = null;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("MainCamera"))
            {
                enterAction?.Invoke();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("MainCamera"))
            {
                exitAction?.Invoke();
            }
        }
    }
}
