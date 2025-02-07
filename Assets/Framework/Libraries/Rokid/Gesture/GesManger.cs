using System;
using System.Collections.Generic;
using System.Linq;
using Rokid.UXR.Interaction;
using UnityEngine;

namespace XY.UXR
{
    public class GesManger : GestureBase
    {
        public static GesManger Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            //else
            //{
            //    Destroy(gameObject);
            //}
            Debug.Log("---------->>> ges awake");
        }

        #region Test

        [SerializeField] private GameObject m_1;
        [SerializeField] private GameObject m_2;
        [SerializeField] private GameObject m_3;
        [SerializeField] private GameObject m_4;
        [SerializeField] private GameObject m_root;

        #endregion


        /// <summary>
        /// 旋转角度，手心位置,手掌up向量
        /// </summary>
        public Action<Quaternion?, Vector3?, Vector3?, int> OnTransform;


        /// <summary>
        /// 检测手掌情况下手的个数
        /// </summary>
        public Action<int> OnHandNumAction;

        /// <summary>
        /// 当前手势获取
        /// </summary>
        public GestureBean m_gestureBean;

        /// <summary>
        /// 手掌队列 key=> 手掌类型， value => 是否跟踪 (0没有跟踪，1跟踪，2跟踪并是手掌 )
        /// </summary>
        private Dictionary<int, int> m_trackedDic = new Dictionary<int, int>();

        /// <summary>
        /// 手掌x轴角度
        /// </summary>
        public Vector2 handUp = new Vector2(-5, 60);

        /// <summary>
        /// 手掌z轴角度
        /// </summary>
        public Vector2 handLR = new Vector2(-40, 70);


        public void OnEnable()
        {
            BindEvent();
        }

        public void OnDisable()
        {
            UnBindEvent();
        }

        #region HandEvent
        public override void OnTrackedSuccess(HandType type)
        {
            if (type == HandType.None)
            {
                return;
            }

            if (m_trackedDic.ContainsKey((int)type))
            {
                m_trackedDic[(int)type] = m_trackedDic[(int)type] < 1 ? 1 : m_trackedDic[(int)type];
                return;
            }

            m_trackedDic.Add((int)type, 1);
        }
        public override void OnTrackedFailed(HandType type)
        {
            if (m_trackedDic.ContainsKey((int)type))
            {
                var item = m_trackedDic[(int)type];
                if (item == 2)
                {
                    OnLossTrackedHandUpDeal(item);
                }

                m_trackedDic[(int)type] = 0;
            }


            if (type == HandType.None)
            {
                OnTransform.Invoke(null, null, null, 0);
                m_trackedDic.Clear();
            }
        }
        public override void OnRenderHand(HandType arg1, GestureBean bean)
        {
            if (arg1 == HandType.None)
            {
                OnTransform?.Invoke(null, null, null, 0);
                foreach (int key in m_trackedDic.Keys)
                {
                    m_trackedDic[key] = 0;
                }

                return;
            }

            if (bean == null)
            {
                return;
            }
#if UNITY_EDITOR
            if(m_root!=null)
            {
                m_root.SetActive(true);
                Vector3[] vector3s = new Vector3[18];
                vector3s[17] = m_4.transform.position;
                vector3s[0] = m_1.transform.position;
                vector3s[2] = m_2.transform.position;
                vector3s[9] = m_3.transform.position;
                bean.skeletons = vector3s;
                bean.hand_orientation = 0;
            }
#endif

            if (m_trackedDic.ContainsValue(2))
            {
                //有手掌
                var t = m_trackedDic.First(d => d.Value == 2);
                if (bean.hand_type != t.Key)
                {
                    m_trackedDic[(int)arg1] = 1;
                    return;
                }

                var a = isHandUp(bean);
                if (a.Item1)
                {
                    OnTrackedHandUpDeal(bean, a.Item2, a.Item3);
                    return;
                }

                OnLossTrackedHandUpDeal(bean.hand_type);
                return;
            }

            //检测手掌

            if (bean.hand_orientation == 0)
            {
                var u = isHandUp(bean);
                if (!u.Item1) return;
                OnTrackedHandUpDeal(bean, u.Item2, u.Item3);
                return;
            }

            m_trackedDic[bean.hand_type] = 1;
        }
        #endregion

        /// <summary>
        /// 满足触发条件 
        /// </summary>
        /// <param name="gestureBean"></param>
        /// <returns></returns>
        private (bool, Quaternion, Vector3) isHandUp(GestureBean gestureBean)
        {
            if (gestureBean == null || gestureBean.skeletons == null || gestureBean.skeletons.Count() < 18)
            {
                return (false, Quaternion.identity, Vector3.one);
            }

            var thumbBase = gestureBean.skeletons[9];
            var pinkBase = gestureBean.skeletons[17];
            var wrist = gestureBean.skeletons[2];
            var q = PositionUtils.HanQuaternion(thumbBase, pinkBase, wrist, gestureBean.hand_type);
            var euler = q.Item2.eulerAngles;
            var z = PositionUtils.ConvertTo180(euler.z);
            var x = PositionUtils.ConvertTo180(euler.x);
            if (gestureBean.hand_type == 1)
            {
                return (-handLR.y < z &&
                        -handLR.x > z &&
                        handUp.x < x &&
                        handUp.y > x, q.Item2, q.Item1);
            }

            return (handLR.x < z &&
                    handLR.y > z &&
                    handUp.x < x &&
                    handUp.y > x, q.Item2, q.Item1);
        }


        /// <summary>
        /// 获取掌心位置
        /// </summary>
        /// <param name="gestureBean"></param>
        /// <returns></returns>
        private Vector3 GetHandCenter(GestureBean gestureBean)
        {
            if (gestureBean == null || gestureBean.skeletons == null || gestureBean.skeletons.Count() < 18)
            {
                return Vector3.one;
            }

            var middle = gestureBean.skeletons[9];
            var wrist = gestureBean.skeletons[0];
            return (wrist + middle) / 2;
        }

        private int tem = 0;
        private void Update()
        {

#if UNITY_EDITOR
            OnRenderHand(HandType.LeftHand, new GestureBean());
            OnHandNumAction?.Invoke(tem);
            if (Input.GetKeyDown(KeyCode.P))
            {
                tem = 1;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                tem = 2;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                tem = 0;
            }
            return;
#endif

            OnHandNumAction?.Invoke(m_trackedDic.Count(kv => kv.Value >= 1));
        }

        private void OnLossTrackedHandUpDeal(int handType)
        {
            m_trackedDic[handType] = 1;
            OnTransform?.Invoke(null, null, null, handType);
            m_gestureBean = null;
        }

        private void OnTrackedHandUpDeal(GestureBean gestureBean, Quaternion quaternion, Vector3 up)
        {
            m_trackedDic[gestureBean.hand_type] = 2;
            OnTransform?.Invoke(quaternion, GetHandCenter(gestureBean), up.normalized, gestureBean.hand_type);
            m_gestureBean = gestureBean;
        }
    }
}