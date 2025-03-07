using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR.Gesture
{
    /// <summary>
    /// 手掌姿态事件数据结构
    /// </summary>
    //public class PalmEvent
    //{
    //    public bool status;           // 手掌状态
    //    public Vector3 position;      // 手掌位置
    //    public Vector3 eulerAngles;   // 手掌旋转角度
    //    public HandType handType;     // 手的类型（左手/右手）
    //}

    /// <summary>
    /// 手掌手势识别控制器
    /// 用于识别和处理手掌相关的手势
    /// </summary>
    public class GesPalm1 : GestureBase
    {
        private PalmEvent palmEvent = new PalmEvent();    // 手掌事件数据

        private GestureBean leftBean = null;              // 左手手势数据
        private GestureBean rightBean = null;             // 右手手势数据
        public GameObject mAxis = null;                   // 手掌轴向显示预制体
        private GameObject axisItem = null;               // 实例化的轴向显示对象
        public bool IsPinching = false;                   // 是否正在进行捏合手势

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            // 实例化轴向显示对象
            if (mAxis != null)
            {
                axisItem = Instantiate(mAxis);
            }
            BindEvent();
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            UnBindEvent();
        }

        /// <summary>
        /// 手势追踪失败回调
        /// </summary>
        /// <param name="handType">手的类型</param>
        public override void OnTrackedFailed(HandType handType)
        {
            base.OnTrackedFailed(handType);
            // 根据失败的手类型清除对应的手势数据
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

        /// <summary>
        /// 手势渲染更新回调
        /// </summary>
        /// <param name="handType">手的类型</param>
        /// <param name="gestureBean">手势数据</param>
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);
            // 更新对应手的手势数据
            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
            }
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
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

            // 更新手掌事件数据
            palmEvent.status = mPose != Pose.identity;
            palmEvent.position = mPose.position;
            palmEvent.eulerAngles = mPose.rotation.eulerAngles;

            // 在非编辑器模式下发送手掌事件
#if !UNITY_EDITOR   // 真机测试
            MessageDispatcher.SendMessageData(API.OpenAPI.RKGesPalmEvent1, palmEvent);
#endif

            // 更新轴向显示对象的位置和旋转
            if (axisItem != null && !IsPinching)
            {
                axisItem.SetActive(palmEvent.status);
                axisItem.transform.position = palmEvent.position;
                axisItem.transform.eulerAngles = palmEvent.eulerAngles;
            }
        }
    }
}