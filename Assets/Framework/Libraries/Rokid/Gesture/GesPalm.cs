using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR.Gesture
{
    public class PalmEvent
    {
        public bool status;
        public Vector3 position;
        public Vector3 eulerAngles;
        public HandType handType;
    }
    public class GesPalm : GestureBase
    {
        private PalmEvent palmEvent = new PalmEvent();

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
            Pose mPose = Pose.identity;
            palmEvent.handType = HandType.None;
            if (leftBean != null && (HandOrientation)leftBean.hand_orientation == HandOrientation.Palm && (GestureType)leftBean.gesture_type == GestureType.Palm)
            {
                mPose = GesEventInput.Instance.GetHandPose(HandType.LeftHand);
                palmEvent.handType = HandType.LeftHand;
            }
            if (rightBean != null && (HandOrientation)rightBean.hand_orientation == HandOrientation.Palm && (GestureType)rightBean.gesture_type == GestureType.Palm)
            {
                mPose = GesEventInput.Instance.GetHandPose(HandType.RightHand);
                palmEvent.handType = HandType.RightHand;
            }
            palmEvent.status = mPose != Pose.identity;
            palmEvent.position = mPose.position;
            palmEvent.eulerAngles = mPose.rotation.eulerAngles;
            MessageDispatcher.SendMessageData(API.OpenAPI.RKGesPalmEvent, palmEvent);
        }
    }
}