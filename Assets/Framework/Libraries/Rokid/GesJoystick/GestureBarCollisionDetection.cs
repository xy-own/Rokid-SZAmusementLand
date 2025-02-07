using Rokid.UXR.Interaction;
using UnityEngine;
using XY.UXR;

namespace XY.UXR
{
    public class GestureBarCollisionDetection : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            //只检测碰撞进入，结束推拉手势的检测不是由碰撞检测来决定的，是由超出一定的阈值来决定的
            if (collider.gameObject.name.Equals("Handle"))
            {
                if (name.StartsWith("left"))
                {
                    if (GestureBarHelper.Instance.m_handType == HandType.None)
                    {
                        GestureBarHelper.Instance.m_handType = HandType.LeftHand;
                    }
                }
                else if (name.StartsWith("right"))
                {
                    if (GestureBarHelper.Instance.m_handType == HandType.None)
                    {
                        GestureBarHelper.Instance.m_handType = HandType.RightHand;
                    }
                }
                if (GestureBarHelper.Instance.m_collisionPoint == Vector3.zero)
                {
                    GestureBarHelper.Instance.m_collisionPoint = collider.ClosestPointOnBounds(collider.gameObject.transform.position);
                }
            }
        }
    }
}