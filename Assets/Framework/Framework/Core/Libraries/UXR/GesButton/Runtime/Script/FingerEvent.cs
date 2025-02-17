using UnityEngine;
using XY.UXR.Gesture.Trigger;
namespace XY.UXR.Gesture.Button
{
    public class FingerEvent : MonoBehaviour
    {
        private int handCount = 1;
        public void SetHandCount(int count)
        {
            if (count != handCount)
            {
                handCount = count;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BtnItem btn))
            {
                btn.Enter(this, other);
            }
            if (other.gameObject.TryGetComponent(out TriggerItem trigger))
            {
                trigger.Enter(gameObject.transform.position, handCount);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BtnItem btn))
            {
                btn.Exit(this, other);
            }
            if (other.gameObject.TryGetComponent(out TriggerItem trigger))
            {
                trigger.Exit(gameObject.transform.position, handCount);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BtnItem btn))
            {
                btn.Stay(this, other);
            }
        }
    }
}