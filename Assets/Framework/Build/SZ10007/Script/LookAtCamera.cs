using Rokid.UXR.Utility;
using UnityEngine;

namespace SU10007
{
    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            Vector3 forward = transform.position - Camera.main.transform.position;
            forward.y = 0;
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}

