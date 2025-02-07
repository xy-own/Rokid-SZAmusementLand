using System;
using Rokid.UXR.Interaction;
using TMPro;
using UnityEngine;

namespace XY.UXR
{
    public class GestureBarHelper : GestureBase
    {
        
        public TextMeshProUGUI text;
        
        private const string TAG = "GestureBarHelper";

        public static GestureBarHelper Instance;
        
        private TimeMachineHandle m_machineHandle;

        /// <summary>
        /// 当前正在跟踪的碰撞检测的手
        /// </summary>
        public HandType m_handType;

        // 首次碰撞点
        public Vector3 m_collisionPoint = Vector3.zero;

        // 推拉角度的回调
        public Action<float> onRotate;

        // 手势丢失的回调
        public Action onLoss;

        // 手势丢失的最远距离阈值
        public float m_lossDistance = 1f;

        // 拉杆模型的轴点
        private Transform m_transform;
        
        // 手部骨骼处的碰撞体的预制球体，需要动态的生成
        public GameObject m_collider;

        // 动态生成的碰撞检测体,左手和右手可能同时存在
        private GameObject left_6;
        private GameObject left_10;
        private GameObject left_14;
        private GameObject right_6;
        private GameObject right_10;
        private GameObject right_14;

        private bool isCalculate;

        public void init(TimeMachineHandle machineHandle)
        {
            m_machineHandle = machineHandle;
            m_transform = m_machineHandle.transform;
            BindEvent();
        }

        public void deinit()
        {
            m_machineHandle = null;
            m_transform = null;
            m_collider = null;
            onDetectionLoss();
            UnBindEvent();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public override void OnTrackedFailed(HandType type)
        {
            if (m_handType != HandType.None && m_handType == type)
            {
                Log("OnTrackedFailed");
                if (type == HandType.LeftHand)
                {
                    if (left_6 != null)
                        Destroy(left_6);
                    if (left_10 != null)
                        Destroy(left_10);
                    if (left_14 != null)
                        Destroy(left_14);
                }
                else
                {
                    if (right_6 != null)
                        Destroy(right_6);
                    if (right_10 != null)
                        Destroy(right_10);
                    if (right_14 != null)
                        Destroy(right_14);
                }
                onDetectionLoss();
            }
        }

        public override void OnTrackedSuccess(HandType type)
        {
        }

        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            if (gestureBean.hand_orientation == (int)HandOrientation.Back)
            {
                //如果是手背，增加需要检测的3个碰撞体，这里需要取中指的中间，对应的骨骼点是6,10,14
                var gestureBeanSkeletons = gestureBean.skeletons;
                var vector6 = gestureBeanSkeletons[6];
                var vector10 = gestureBeanSkeletons[10];
                var vector14 = gestureBeanSkeletons[14];
                if (handType == HandType.LeftHand)
                {
                    if (left_6 == null && left_10 == null && left_14 == null)
                    {
                        left_6 = Instantiate(m_collider, vector6, Quaternion.identity);
                        left_6.name = "left_6";
                                                    
                        left_10 = Instantiate(m_collider, vector10, Quaternion.identity);
                        left_10.name = "left_10";
                        
                        left_14 = Instantiate(m_collider, vector14, Quaternion.identity);
                        left_14.name = "left_14";
                    }
                    else
                    {
                        left_6.transform.position = vector6;
                        left_10.transform.position = vector10;
                        left_14.transform.position = vector14;
                    }
                }
                else
                {
                    if (right_6 == null && right_10 == null && right_14 == null)
                    {
                        right_6 = Instantiate(m_collider, vector6, Quaternion.identity);
                        right_6.name = "right_6";
                        
                        right_10 = Instantiate(m_collider, vector10, Quaternion.identity);
                        right_10.name = "right_10";
                        
                        right_14 = Instantiate(m_collider, vector14, Quaternion.identity);
                        right_14.name = "right_14";
                    }
                    else
                    {
                        right_6.transform.position = vector6;
                        right_10.transform.position = vector10;
                        right_14.transform.position = vector14;
                    }
                }
            }
            else
            {
                if (handType == m_handType)
                {
                    Log("back hand dismiss");
                    onDetectionLoss();
                }

                if (handType == HandType.LeftHand)
                {
                    if (left_6 != null)
                        Destroy(left_6);
                    if (left_10 != null)
                        Destroy(left_10);
                    if (left_14 != null)
                        Destroy(left_14);
                }
                else
                {
                    if (right_6 != null)
                        Destroy(right_6);
                    if (right_10 != null)
                        Destroy(right_10);
                    if (right_14 != null)
                        Destroy(right_14);
                }
            }

            //握拳
            if (gestureBean.gesture_type == 1)
            {
                if (handType!= HandType.None && m_handType == handType)
                {
                    //如果检测到碰撞，则需要一直计算推拉的角度
                    calculateAngle(gestureBean);
                }
            }
            else
            {
                //变为其他非握拳姿态
                if (m_handType != HandType.None && m_handType == handType)
                {
                    if (isCalculate)
                    {
                        Log("not fist");
                        onDetectionLoss();
                    }
                }
            }
        }

        private void calculateAngle(GestureBean gestureBean)
        {
            isCalculate = true;
            var currentPoint0 = gestureBean.skeletons[0];
            var currentPoint9 = gestureBean.skeletons[9];
            var current = (currentPoint0 + currentPoint9) / 2;
            var distance = Vector3.Distance(current, m_collisionPoint);
            //如果当前点与首次碰撞检测的点的距离超过一定的阈值，判定退出检测
            if (distance > m_lossDistance)
            {
                Log("loss distance");
                onDetectionLoss();
            }
            else
            {
                // 法线向量
                var targetForward =  m_transform.right;
                // 轴点到起始点的向量
                var startVector =  m_collisionPoint - m_transform.position;
                // 轴点到当前点的向量
                var currentVector = current - m_transform.position;
                // 计算两个向量投影到法线向量上的夹角
                var angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(startVector, targetForward), Vector3.ProjectOnPlane(currentVector, targetForward),targetForward);
                Log("angle:"+angle);
                onRotate?.Invoke(angle);
            }
        }

        private void onDetectionLoss()
        {
            m_handType = HandType.None;
            m_collisionPoint = Vector3.zero;
            isCalculate = false;
            onLoss?.Invoke();
        }

        private void Log(string msg)
        {
            if (text != null)
            {
                text.text = msg;
            }

            Debug.Log(TAG + "--->: " + msg);
        }

    }
}
