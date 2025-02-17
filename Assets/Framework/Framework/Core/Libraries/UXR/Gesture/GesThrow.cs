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

    public class GesThrow : GestureBase
    {
        private ThrowInfo throwInfo = new ThrowInfo();
        private bool posRecords = false;
        private Vector3 oldPos = Vector3.zero;
        private long duration = 0L;

        private GestureBean leftBean = null;
        private GestureBean rightBean = null;

        [Header("参考点 默认为相机")]
        public Transform referencePoint = null;

        private void Awake()
        {
            if(referencePoint==null)
            {
                referencePoint = Camera.main.transform;
            }
            BindEvent();
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
            GestureBean bean = null;
            if (leftBean != null)
            {
                bean = leftBean;
            }
            if (rightBean != null)
            {
                bean = rightBean;
            }
            HandEvent(bean);
        }
        private void HandEvent(GestureBean bean)
        {
            if (bean != null)
            {
                // 食指和大拇指的中间点 取捏合的位置
                Vector3 midPoint = Vector3.Lerp(bean.skeletons[(int)SkeletonIndexFlag.THUMB_TIP], bean.skeletons[(int)SkeletonIndexFlag.INDEX_FINGER_TIP], 0.5f);

                // 食指指尖 中指根部 手腕 三点成一个面 取面的向量
                Vector3 p1 = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.INDEX_FINGER_TIP, (HandType)bean.hand_type).position;
                Vector3 p2 = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.MIDDLE_FINGER_MCP, (HandType)bean.hand_type).position;
                Vector3 p3 = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, (HandType)bean.hand_type).position;
                Vector3 normal = Vector3.Cross((p3 - p1), (p2 - p1)).normalized;

                float offsetNormal = -10f;
                // 左手时需要对向量取反
                if ((HandType)bean.hand_type == HandType.LeftHand)
                {
                    normal *= -1;
                    offsetNormal *= -1;
                }
                normal = Quaternion.AngleAxis(offsetNormal, Vector3.up) * normal;

                if (GesEventInput.Instance.GetHandPress((HandType)bean.hand_type))
                {
                    if (!posRecords)
                    {
                        posRecords = true;

                        duration = Util.GetTime();
                        oldPos = midPoint;
                    }
                    else if (Vector3.Distance(referencePoint.position, midPoint) < Vector3.Distance(referencePoint.position, oldPos)|| Vector3.Distance(midPoint, oldPos)<0.02f)
                    {
                        duration = Util.GetTime();
                        oldPos = midPoint;
                    }
                    SetThrowInfo(ThrowStatus.Tracking, midPoint, normal);
                }
                else
                {
                    if (posRecords)
                    {
                        posRecords = false;
                        Vector3 position = midPoint;
                        Vector3 dis = (position - oldPos).normalized;
                        long time = Util.GetTime() - duration;
                        ThrowStatus api = Vector3.Distance(position, oldPos)>0.05f?ThrowStatus.Action: ThrowStatus.None;
                        SetThrowInfo(api, position, dis, time);
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
    }
}