using UnityEngine;
namespace D
{
    public class FingerEvent : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BtnItem btn))
            {
                btn.Enter();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BtnItem btn))
            {
                btn.Exit();
            }
        }
    }
}