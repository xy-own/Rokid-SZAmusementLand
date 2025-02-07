using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR
{
    public class GestureBase : MonoBehaviour, IUXRHandEvent
    {
        public void BindEvent()
        {
            GestureManager.AddManager(this);
        }
        public void UnBindEvent()
        {
            GestureManager.RemoveManager(this);
        }
        public virtual void OnTrackedSuccess(HandType handType)
        {
        }
        public virtual void OnTrackedFailed(HandType handType)
        {
        }
        public virtual void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
        }
    }
}