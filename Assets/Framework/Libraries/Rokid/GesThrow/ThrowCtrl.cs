using System;
using UnityEngine;
using Rokid.UXR.Interaction;

namespace XY.UXR.Gesture
{
    public enum ThrowStatus
    {
        /// <summary>
        /// 非投掷
        /// </summary>
        None,
        /// <summary>
        /// 投掷手势追踪
        /// </summary>
        Tracking,
        /// <summary>
        /// 完成一次投掷动作
        /// </summary>
        Action
    }
    public class ThrowInfo
    {
        /// <summary>
        /// 当前手势状态
        /// </summary>
        public ThrowStatus status;
        /// <summary>
        /// 食指与大拇指的位置
        /// </summary>
        public Vector3? position;
        /// <summary>
        /// 食指与大拇指的旋转向量
        /// </summary>
        public Vector3? normal;
        /// <summary>
        /// 当前投掷动作的持续时间 毫秒
        /// </summary>
        public long duration;
    }

    public class ThrowCtrl : GestureBase
    {
        private ThrowInfo throwInfo = new ThrowInfo();
        private Vector3 oldPos = Vector3.zero;
        private long duration = 0L;

        private GestureBean leftBean = null;
        private GestureBean rightBean = null;

        public bool isMock = false;
        private LineRenderer mLine = null;
        private void Awake()
        {
            BindEvent();
            if (isMock)
            {
                mLine = new GameObject("ThrowMock").AddComponent<LineRenderer>();
                mLine.startWidth = 0.01f;
            }
        }
        private void OnDestroy()
        {
            UnBindEvent();
        }

        public override void OnTrackedSuccess(HandType handType)
        {
            base.OnTrackedSuccess(handType);
            if (handType == HandType.None)
                return;
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
        /// <summary>
        /// 该函数本身也是由Update驱动 所以尽量只做数据的提取和保存 不写业务逻辑
        /// </summary>
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);
            if (handType == HandType.LeftHand)
            {
                leftBean = (HandOrientation)gestureBean.hand_orientation == HandOrientation.Back ? gestureBean : null;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = (HandOrientation)gestureBean.hand_orientation == HandOrientation.Back ? gestureBean : null;
            }
            //ThrowUpdate();
        }
       
        private void Update()
        {
            GestureBean bean = null;
            if (leftBean != null)
            {
                GestureType type = (GestureType)leftBean.gesture_type;
                if (type == GestureType.Pinch || type == GestureType.Palm)
                {
                    bean = leftBean;
                }
            }
            // 默认右手 所以不能使用else 右手逻辑在后直接覆盖
            if (rightBean != null)
            {
                GestureType type = (GestureType)rightBean.gesture_type;
                if (type == GestureType.Pinch || type == GestureType.Palm)
                {
                    bean = rightBean;
                }
            }
            HandEvent(bean);
        }
        private void HandEvent (GestureBean bean)
        {
            if (bean != null)
            {
                // 食指坐标
                Vector3 forefinger = bean.skeletons[8];
                // 大拇指坐标
                Vector3 thumb = bean.skeletons[4];
                // position
                Vector3 midPoint = Vector3.Lerp(thumb, forefinger, 0.5f);

                Vector3 p1 = bean.skeletons[8];
                Vector3 p2 = bean.skeletons[4];
                Vector3 p3 = bean.skeletons[3];
                //Vector3 panelPos = (p1 + p2 + p3) / 3f;
                Vector3 normal = Vector3.Cross((p2 - p1), (p3 - p1)).normalized;
                //if (isMock)
                //{
                //    mLine.SetPosition(0, panelPos);
                //    mLine.SetPosition(1, normal);
                //}

                GestureType type = (GestureType)bean.gesture_type;
                if (type == GestureType.Pinch)
                {
                    if (throwInfo.status != ThrowStatus.Tracking)
                    {
                        duration = Util.GetTime();
                        oldPos = midPoint;
                        if (isMock)
                        {
                            mLine.SetPosition(0, midPoint);
                        }
                    }
                    Log("Pinch", "Tracking");
                    SetThrowInfo(ThrowStatus.Tracking, midPoint, normal);
                }
                if (type == GestureType.Palm)
                {
                    if (throwInfo.status == ThrowStatus.Tracking)
                    {
                        long time = Util.GetTime() - duration;
                        Vector3 position = midPoint;
                        Vector3 nor = (position - oldPos).normalized;
                        if (isMock)
                        {
                            mLine.SetPosition(1, midPoint);
                        }

                        Log("Palm", "Tracking");
                        SetThrowInfo(ThrowStatus.Action, position, nor, time);
                    }
                }
            }
            else if (throwInfo.status != ThrowStatus.None)
            {
                SetThrowInfo(ThrowStatus.None);
            }
        }
        private void SetThrowInfo(ThrowStatus status, Vector3? position = null, Vector3? normal = null, long duration = 0L)
        {
            throwInfo.status = status;
            throwInfo.position = position;
            throwInfo.normal = normal;
            throwInfo.duration = duration;
            MessageDispatcher.SendMessageData(API.OpenAPI.RKGesThrow, throwInfo);
        }

        int index = 0;
        string mTag = string.Empty;
        private void Log(string path, string info)
        {
            if (path != mTag)
            {
                mTag = path;
                index = 0;
            }
            index++;
            if (index >= 100)
            {
                index = 0;
            }
            //Main.sdkMgr.VoiceMsg($" ------->>>{path}   {info}   {index}");
        }
    }
}