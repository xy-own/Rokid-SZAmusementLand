using UnityEngine;

namespace XY.UXR
{
    public class DepthButtonCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var parent = other.gameObject.transform.parent;
            if (parent != null)
            {
                var custom = parent.GetComponent<Custom>();
                if (custom != null)
                {
                    custom.OnCustomTriggerEnter(other);
                }
                if (other.gameObject.TryGetComponent(out ObjTrigger trigger))
                {
                    trigger.Enter(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var parent = other.gameObject.transform.parent;
            if (parent != null)
            {
                var custom = parent.GetComponent<Custom>();
                if (custom != null)
                {
                    custom.OnCustomTriggerExit(other);
                }
                if (other.gameObject.TryGetComponent(out ObjTrigger trigger))
                {
                    trigger.Exit(other);
                }
            }
        }
    }
}
