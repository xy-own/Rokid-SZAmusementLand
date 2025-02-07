using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XY.TriggerEvent
{
    public class OpenAPI
    {
        public const string TriggerEnter = "EventTriggerEnter";
        public const string TriggerExit = "EventTriggerExit";
    }
    public class ColliderTrigger : MonoBehaviour
    {
        Player mPlayer;
        public LayerMask triggerTag = 0 << 1;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == triggerTag.value)
            {
                if (other.TryGetComponent(out mPlayer))
                {
                    MessageDispatcher.SendMessageData(OpenAPI.TriggerEnter, gameObject);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == triggerTag.value)
            {
                if (other.TryGetComponent(out mPlayer))
                {
                    MessageDispatcher.SendMessageData(OpenAPI.TriggerExit, gameObject);
                }
            }
        }
    }
}

