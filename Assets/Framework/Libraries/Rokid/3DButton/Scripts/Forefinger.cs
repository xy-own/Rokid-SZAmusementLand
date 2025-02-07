using System;
using Rokid.UXR;
using Rokid.UXR.Interaction;
using TMPro;
using UnityEngine;

namespace XY.UXR
{
    public class Forefinger : GestureBase
    {
        public static Forefinger Instance;

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

        //当前正在跟踪的手
        private HandType m_handType = HandType.None;

        //食指指尖的预制体，用来做碰撞检测
        public GameObject m_collider;
        private GameObject right_obj8;
        private GameObject left_obj8;

        private Custom m_custom;

        private void OnEnable()
        {
            BindEvent();
        }

        public void OnDisable()
        {
            UnBindEvent();
            m_handType = HandType.None;
            m_custom?.onLoss(0);
            m_custom = null;
        }

        public override void OnTrackedFailed(HandType type)
        {
            if (type == HandType.None)
            {
                m_custom?.onLoss(1);
                m_custom = null;
                m_handType = HandType.None;
                destory(type);
            }
            else
            {
                if (m_handType == type)
                {
                    m_custom?.onLoss(1);
                    m_custom = null;
                    m_handType = HandType.None;
                    destory(type);
                }
            }
        }

        private void destory(HandType type)
        {
            if (type == HandType.RightHand)
            {
                if (right_obj8 != null)
                {
                    Destroy(right_obj8);
                }
            }
            else if (type == HandType.LeftHand)
            {
                if (left_obj8 != null)
                {
                    Destroy(left_obj8);
                }
            }
        }

        public override void OnTrackedSuccess(HandType type)
        {
        }

        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            if (gestureBean == null)
                return;
            if (gestureBean.hand_orientation == (int)HandOrientation.Back)
            {
                //如果是手背，需要在食指的2个骨骼用做射线检测
                Vector3[] gestureBeanSkeletons = gestureBean.skeletons;
                Vector3 vector7 = gestureBeanSkeletons[7];
                Vector3 vector8 = gestureBeanSkeletons[8];
                //vector7指向vector8的向量
                Vector3 vector7_8 = vector8 - vector7;
                //vector8做碰撞检测
                if (handType == HandType.LeftHand)
                {
                    if (left_obj8 == null)
                    {
                        left_obj8 = Instantiate(m_collider, vector8, Quaternion.identity);
                        left_obj8.name = "left_obj8";
                    }
                    else
                    {
                        //更新位置
                        left_obj8.transform.position = vector8;
                    }
                }
                else if (handType == HandType.RightHand)
                {
                    if (right_obj8 == null)
                    {
                        right_obj8 = Instantiate(m_collider, vector8, Quaternion.identity);
                        right_obj8.name = "right_obj8";
                    }
                    else
                    {
                        //更新位置
                        right_obj8.transform.position = vector8;
                    }
                }
                if (m_handType == HandType.None || m_handType == handType)
                {
                    //由这个向量向top发出一个射线，检测碰撞并判断射线的长度是否小于hitDistance
                    Ray ray = new Ray(vector8, vector7_8);
                    if (Physics.Raycast(ray, out var hit))
                    {
                        var parent = hit.collider.transform.parent;
                        if (parent != null)
                        {
                            var custom = parent.GetComponent<Custom>();
                            if (custom != null)
                            {
                                if (m_custom != custom)
                                {
                                    m_custom?.onLoss(2);
                                    m_custom = custom;
                                }
                                CheckOnPress(vector8, vector7_8);
                            }
                            else
                            {
                                m_custom?.onLoss(3);
                                m_custom = null;
                            }
                        }
                        if (m_custom != null)
                        {
                            m_custom?.onDistanceChange(hit.distance, handType);
                        }
                        else
                        {
                            if (m_handType == handType)
                            {
                                m_custom?.onLoss(4);
                                m_custom = null;
                                m_handType = HandType.None;
                            }
                        }
                    }
                    else
                    {
                        m_custom?.onLossRay();
                    }
                }
            }
            else
            {
                if (m_handType == handType)
                {
                    m_custom?.onLoss(6);
                    m_custom = null;
                    m_handType = HandType.None;
                }
                destory(handType);
            }
        }

        private void CheckOnPress(Vector3 vector8, Vector3 vector7_8)
        {
            //这里检测和 bottom 的距离，来做按压效果
            RaycastHit[] hits = Physics.RaycastAll(vector8, vector7_8);
            if (hits != null)
            {
                foreach (RaycastHit hitOne in hits)
                {
                    GameObject hitObject = hitOne.collider.gameObject;
                    if (hitObject.name == "BottomCollider")
                    {
                        m_custom?.onPress(hitOne.distance);
                    }
                }
            }
        }

        public void setHandType(HandType handType)
        {
            m_handType = handType;
            if (m_handType == HandType.None)
            {
                m_custom = null;
            }
        }

        public HandType getHandType()
        {
            return m_handType;
        }
    }
}