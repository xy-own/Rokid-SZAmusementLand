using Rokid.UXR;
using Rokid.UXR.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XY.UXR.Gesture
{
    /// <summary>
    /// 手掌姿态事件数据结构
    /// </summary>
    public class ScissorsEvent
    {
        public bool status;           // 手掌状态
        public Vector3 position;      // 手掌位置
        public Vector3 eulerAngles;   // 手掌旋转角度
        public HandType handType;     // 手的类型（左手/右手）
    }
    /// <summary>
    /// 剪刀手势
    /// </summary>
    public class GesScissors : MonoBehaviour
    {
        private ScissorsEvent palmInfo = new ScissorsEvent();
        private GestureBean rightBean = null;
        private GestureBean leftBean = null;
        private bool m_IsScissors;
        private Camera mCamera = null;
        private void Awake()
        {
            GesEventInput.OnTrackedSuccess += OnTrackedSuccess;
            GesEventInput.OnTrackedFailed += OnTrackedFailed;
            GesEventInput.OnRenderHand += OnRenderHand;
        }
        private void OnDestroy()
        {
            GesEventInput.OnTrackedSuccess -= OnTrackedSuccess;
            GesEventInput.OnTrackedFailed -= OnTrackedFailed;
            GesEventInput.OnRenderHand -= OnRenderHand;
        }
        void OnTrackedSuccess(HandType handType)
        {
            if (handType == HandType.None)
            {
            }
            if (handType == HandType.RightHand)
            {
            }
        }
        void OnTrackedFailed(HandType handType)
        {
            if (handType != HandType.LeftHand)
            {
                rightBean = null;
            }
        }
        void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
                m_IsScissors = IsScissorsGesture(gestureBean);
                if (m_IsScissors) 
                {
                    MessageDispatcher.SendMessageData(API.OpenAPI.ScissorsEvent);
                }
            }
            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
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
                m_IsScissors = IsScissorsGesture(rightBean);
                palmInfo.position = rightBean.position;
                palmInfo.eulerAngles = rightBean.rotation.eulerAngles;
            }
            else if (leftBean != null)
            {
                if (mCamera != null)
                {
                    bool bl = IsPointOnScreen(mCamera, rightBean.position);
                    palmInfo.status = bl;
                }
                m_IsScissors = IsScissorsGesture(leftBean);
                palmInfo.position = leftBean.position;
                palmInfo.eulerAngles = leftBean.rotation.eulerAngles;
            }
            if (m_IsScissors)
            {
                MessageDispatcher.SendMessageData(API.OpenAPI.ScissorsEvent,palmInfo);
            }
        }

        public bool IsScissorsGesture(GestureBean gestureBean)
        {
            float indexToPalm = Vector3.Distance(gestureBean.skeletons[(int)SkeletonIndexFlag.INDEX_FINGER_TIP], gestureBean.skeletons[(int)SkeletonIndexFlag.PALM]);
            float middleToPalm = Vector3.Distance(gestureBean.skeletons[(int)SkeletonIndexFlag.MIDDLE_FINGER_TIP], gestureBean.skeletons[(int)SkeletonIndexFlag.PALM]);
            float ringToPalm = Vector3.Distance(gestureBean.skeletons[(int)SkeletonIndexFlag.RING_FINGER_TIP], gestureBean.skeletons[(int)SkeletonIndexFlag.PALM]);
            float pinkyToPalm = Vector3.Distance(gestureBean.skeletons[(int)SkeletonIndexFlag.PINKY_TIP], gestureBean.skeletons[(int)SkeletonIndexFlag.PALM]);
            float thumbToPalm = Vector3.Distance(gestureBean.skeletons[(int)SkeletonIndexFlag.THUMB_TIP], gestureBean.skeletons[(int)SkeletonIndexFlag.PALM]);

            bool isIndexMiddleExtended = indexToPalm > 0.1f && middleToPalm > 0.1f;
            bool isOthersFolded = ringToPalm < 0.05f && pinkyToPalm < 0.05f && thumbToPalm < 0.05f;

            return isIndexMiddleExtended && isOthersFolded;
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

