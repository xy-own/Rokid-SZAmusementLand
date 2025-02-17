using Rokid.UXR.Interaction;
using Unity.Mathematics;
using UnityEngine;

namespace XY.UXR.Gesture
{
    public class PinchEvent
    {
        public bool status;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Quaternion rotation;
        public HandType handType;
        public GestureType gestureType;
    }

    public class GesPinch : GestureBase
    {
        private PinchEvent PinchEvent = new PinchEvent();
        private GestureBean leftBean = null;
        private GestureBean rightBean = null;

        public GameObject mAxis = null;
        private GameObject axisItem = null;
        public bool IsRunPinch = false;
        private void Awake()
        {
            if (mAxis != null)
            {
                axisItem = Instantiate(mAxis);
            }
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
        private int updateCount = 0;
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);
            updateCount = 0;
            // 更新手势数据
            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
            }
            else if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
            }
        }

        private void Update()
        {
            if (!IsRunPinch)
            {
                return;
            }
            updateCount++;
            if (updateCount > 5)
            {
                leftBean = null;
                rightBean = null;
                updateCount = 0;
            }
            // 重置 PinchEvent 数据
            PinchEvent.handType = HandType.None;
            Pose mPose = Pose.identity;

            // 检查左右手的捏合状态
            if (IsPinching(HandType.RightHand, rightBean))
            {
                UpdatePinchEvent(HandType.RightHand, ref mPose);
            }
            else if (IsPinching(HandType.LeftHand, leftBean))
            {
                UpdatePinchEvent(HandType.LeftHand, ref mPose);
            }

            PinchEvent.status = PinchEvent.handType != HandType.None;
            // 真机测试时发送事件
#if !UNITY_EDITOR
            MessageDispatcher.SendMessageData(API.OpenAPI.RKGesOpenPinchEvent, PinchEvent);
#endif

            // 更新轴对象
            if (axisItem != null)
            {
                // axisItem.SetActive(PinchEvent.status);
                axisItem.transform.position = PinchEvent.position;
                axisItem.transform.rotation = PinchEvent.rotation;
            }
        }

        /// <summary>
        /// 检查是否在捏合
        /// </summary>
        private bool IsPinching(HandType handType, GestureBean bean)
        {
            if (bean != null)
            {
                if ((GestureType)bean.gesture_type == GestureType.Pinch || (GestureType)bean.gesture_type == GestureType.Grip || (GestureType)bean.gesture_type != GestureType.OpenPinch)
                {
                    // 获取食指指尖的位置和旋转
                    var indexFingerTip = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.INDEX_FINGER_TIP, handType);
                    Vector3 indexFingerPos = indexFingerTip.position;
                    Quaternion indexFingerRot = indexFingerTip.rotation;  // 假设返回的是四元数

                    // 获取拇指指尖的位置和旋转
                    var thumbTip = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.THUMB_TIP, handType);
                    Vector3 thumbPos = thumbTip.position;
                    Quaternion thumbRot = thumbTip.rotation;

                    // 判断手是左手还是右手
                    if (handType == HandType.LeftHand)
                    {
                        Debug.Log(handType + " " + indexFingerRot + thumbRot);
                        // 如果是左手，不需要旋转修正
                        // 左手的旋转保持不变
                    }
                    else if (handType == HandType.RightHand)
                    {
                        Debug.Log(handType + " " + indexFingerRot + thumbRot);
                        // 右手旋转修正
                        Quaternion flipRotation = Quaternion.Euler(180, 180, 180);  // 翻转Y轴
                        indexFingerRot = flipRotation * indexFingerRot;
                        thumbRot = flipRotation * thumbRot;

                        // indexFingerRot = Quaternion.Inverse(indexFingerRot);
                        // thumbRot = Quaternion.Inverse(thumbRot);
                    }
                    // 计算食指和拇指之间的欧几里得距离
                    float distance = Vector3.Distance(indexFingerPos, thumbPos);

                    // 设置一个阈值，例如 0.02 (单位可以根据实际情况调整，通常是米或厘米)
                    float pinchThreshold = 0.05f;

                    // 判断是否是捏取手势
                    if (distance <= pinchThreshold)
                    {
                        PinchEvent.position = (indexFingerPos + thumbPos) / 2; // 捏取位置可以是两点的中点

                        Quaternion handRotation = Quaternion.Lerp(indexFingerRot, thumbRot, 0.5f);
                        PinchEvent.rotation = handRotation;  // 将物体的旋转设置为手的旋转

                        // PinchEvent.eulerAngles = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, handType).rotation.eulerAngles;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                Debug.LogWarning((GestureType)bean.gesture_type);
            }
            return false;
        }

        /// <summary>
        /// 更新 PinchEvent 数据
        /// </summary>
        private void UpdatePinchEvent(HandType handType, ref Pose mPose)
        {
            mPose = GesEventInput.Instance.GetHandPose(handType);
            PinchEvent.handType = handType;
            PinchEvent.gestureType = GestureType.Pinch;
            PinchEvent.status = mPose != Pose.identity;
            // PinchEvent.position = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.INDEX_FINGER_TIP, handType).position;//食指
            // PinchEvent.position = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.THUMB_TIP, handType).position;//拇指
            PinchEvent.eulerAngles = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.INDEX_FINGER_TIP, handType).rotation.eulerAngles;
        }

        /// <summary>
        /// 开始捏合
        /// </summary>
        public void OpenPinch()
        {
            Debug.Log("捏合已开启");
            // 可在此处添加逻辑，如触发特定的动作或事件
        }

        /// <summary>
        /// 停止捏合
        /// </summary>
        public void ClosePinch()
        {
            Debug.Log("捏合已关闭");
            // 可在此处添加逻辑，如停止特定的动作或事件
        }
    }
}
