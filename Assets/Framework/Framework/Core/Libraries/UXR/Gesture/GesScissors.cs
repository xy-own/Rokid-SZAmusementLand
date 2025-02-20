using Rokid.UXR;
using Rokid.UXR.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XY.UXR.Gesture
{
    /// <summary>
    /// 剪刀手势
    /// </summary>
    public class GesScissors : MonoBehaviour
    {
        public Action<bool> action = null;
        private GestureBean rightBean = null;
        private bool m_IsScissors;
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
                m_IsScissors = IsScissorsGesture(gestureBean);
            }
        }
        private void Update()
        {
            //if (m_IsScissors)
            //{
            //    MessageDispatcher.SendMessageData("SmallStarEnter", "P3_earth");
            //    Main.uiCanvas.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = $"触发剪刀手势！！！！！！！";
            //}
            //else
            //{
            //    Main.uiCanvas.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = $"没啦！！！！！！！";
            //}
            //action?.Invoke(m_IsScissors);
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
    }
}

