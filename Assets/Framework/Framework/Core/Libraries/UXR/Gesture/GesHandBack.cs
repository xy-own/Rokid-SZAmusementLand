using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR.Gesture
{
    public class HandBackEvent
    {
        public bool status;
        public Vector3 position;
        public Vector3 eulerAngles;
    }
    public class GesHandBack : GestureBase
    {
        private HandBackEvent palmInfo = new HandBackEvent();
        private GestureBean leftBean = null;
        private GestureBean rightBean = null;

        private void Awake()
        {
            BindEvent();
        }
        private void OnDestroy()
        {
            UnBindEvent();
        }
        public override void OnTrackedFailed(HandType handType)
        {
            base.OnTrackedFailed(handType);
            if (handType == HandType.None)
            {
                leftBean = null;
                rightBean = null;
            }
            if (handType == HandType.LeftHand)
            {
                leftBean = null;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = null;
            }
        }
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);
            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
            }
        }

        private void Update()
        {
            palmInfo.status = false;
            if (leftBean != null && (GestureType)leftBean.gesture_type == GestureType.Palm)
            {
                palmInfo.status = true;
                palmInfo.position = leftBean.position;
                palmInfo.eulerAngles = leftBean.rotation.eulerAngles;
            }
            if (rightBean != null && (GestureType)rightBean.gesture_type == GestureType.Palm)
            {
                palmInfo.status = true;
                palmInfo.position = rightBean.position;
                palmInfo.eulerAngles = rightBean.rotation.eulerAngles;
            }
#if !UNITY_EDITOR
            MessageDispatcher.SendMessageData(API.OpenAPI.RKHandBackEvent, palmInfo);
#endif
        }
    }
}