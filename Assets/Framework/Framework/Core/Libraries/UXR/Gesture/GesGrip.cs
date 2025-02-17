using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR.Gesture
{
    public class GesGrip : GestureBase
    {
        private HandBackEvent palmInfo = new HandBackEvent();
        private GestureBean leftBean = null;
        private GestureBean rightBean = null;
        private Camera mCamera = null;

        private void Awake()
        {
            mCamera = Camera.main;
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
            if (rightBean != null)
            {
                if (mCamera != null)
                {
                    bool bl = IsPointOnScreen(mCamera, rightBean.position);
                    palmInfo.status = bl;
                }

                palmInfo.position = rightBean.position;
                palmInfo.eulerAngles = rightBean.rotation.eulerAngles;
            }
            else if (leftBean != null)
            {
                if (mCamera != null)
                {
                    bool bl = IsPointOnScreen(mCamera, leftBean.position);
                    palmInfo.status = bl;
                }

                palmInfo.position = leftBean.position;
                palmInfo.eulerAngles = leftBean.rotation.eulerAngles;
            }

#if !UNITY_EDITOR
            MessageDispatcher.SendMessageData(API.OpenAPI.RKHandGripEvent, palmInfo);
#endif
        }

        private static bool IsPointOnScreen(Camera camera, Vector3 worldPosition)
        {
            Vector3 viewportPosition = camera.ScreenToViewportPoint(camera.WorldToScreenPoint(worldPosition));
            return viewportPosition.x >= 0f && viewportPosition.x <= 1f &&
                   viewportPosition.y >= 0f && viewportPosition.y <= 1f &&
                   viewportPosition.z > 0f;
        }
    }
}