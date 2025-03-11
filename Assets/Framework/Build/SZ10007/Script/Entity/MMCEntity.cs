/*
* 文件名：NPCEntity.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/19 11:17:56
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using D.Utility;
using DG.Tweening;
using UnityEngine;

namespace SU10007
{
    // 定义动画类型枚举
    public enum MMCAnimationType : byte
    {
        Appear1,
        Appear2,
        Disappear,
        Call,
        FlyR,
        FlyL,
    }
    /// <summary>
    /// 类：NPCEntity
    /// 描述：此类的功能和用途...
    /// </summary>
    public class MMCEntity : MonoBehaviour
    {
        private Animator animator;
        public Transform AttackPoint;
        [Header("被攻击次数")]
        public int BeattackCount = 0;
        [Header("2阶段攻击次数触发")]
        public int Step2Index = 10;
        // 添加一个私有变量来跟踪上次播放被击动画的时间
        private float lastBeattackTime = -1f;
        // 被击动画冷却时间(秒)
        private const float BEATTACK_COOLDOWN = 0.65f;
        public AudioManager m_AudioManager;             // 音频管理器引用
        private Node10007 m_Node10007;

        // 动画参数名称
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int BowIdleHash = Animator.StringToHash("BowIdle");
        private static readonly int BowIdleSpeakHash = Animator.StringToHash("BowIdleSpeak");
        private static readonly int BeattackHash = Animator.StringToHash("Beattack");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int Wag1Hash = Animator.StringToHash("Wag1");
        private static readonly int Wag2Hash = Animator.StringToHash("Wag2");
        private static readonly int Speak1Hash = Animator.StringToHash("Speak1");
        private static readonly int Speak2Hash = Animator.StringToHash("Speak2");
        private static readonly int Speak3Hash = Animator.StringToHash("Speak3");
        private static readonly int Speak4Hash = Animator.StringToHash("Speak4");
        private static readonly int Speak5Hash = Animator.StringToHash("Speak5");
        /// <summary>
        /// 出现1
        /// </summary>
        private static readonly string Appear1Name = "TY_appear01";
        /// <summary>
        /// 出现2
        /// </summary>
        private static readonly string Appear2Name = "TY_appear02";
        /// <summary>
        /// 消失
        /// </summary>
        private static readonly string DisappearName = "TY_disappear";
        /// <summary>
        /// 打招呼
        /// </summary>
        private static readonly string CallName = "TY_call";
        /// <summary>
        /// 左飞
        /// </summary>
        private static readonly string flyLName = "TY_flyL";
        /// <summary>
        /// 右飞
        /// </summary>
        private static readonly string flyRName = "TY_flyR";
        private static readonly string IdleSpeak01Name = "TY_idlespeak01";
        private static readonly string IdleSpeak02Name = "TY_idlespeak02";
        private static readonly string Speak1Name = "TY_speak01";
        private static readonly string Speak2Name = "TY_speak02";
        private static readonly string Speak3Name = "TY_speak03";
        private static readonly string Speak4Name = "TY_speak04";
        private static readonly string Speak5Name = "TY_speak05";
        /// <summary>
        /// 名牌
        /// </summary>
        private SpriteRenderer Titel;
        public Transform LookTarget;
        public bool isLooatPlayer = true;

        // 旋转持续时间，单位：秒
        [Range(0.1f, 2f)]
        public float rotationDuration = 0.3f;

        // 旋转的Tween对象引用，用于控制和取消旋转
        private Tween rotationTween;

        void Awake()
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }

        public void Update()
        {
            if (isLooatPlayer && LookTarget != null)
            {
                // 只旋转Y轴的方法
                Vector3 targetPosition = LookTarget.position;
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0; // 忽略Y轴高度差异，只在水平面旋转

                if (direction != Vector3.zero) // 防止零向量导致的旋转问题
                {
                    // 创建只有Y轴旋转的四元数
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // 创建仅包含Y轴旋转的目标欧拉角
                    Vector3 targetEuler = new Vector3(
                        transform.rotation.eulerAngles.x,
                        targetRotation.eulerAngles.y,
                        transform.rotation.eulerAngles.z
                    );

                    // 如果已经有正在执行的旋转动画，先停止它
                    if (rotationTween != null && rotationTween.IsActive())
                    {
                        rotationTween.Kill();
                    }

                    // 使用DOTween创建旋转动画
                    rotationTween = transform.DORotate(targetEuler, rotationDuration)
                        .SetEase(Ease.OutSine) // 设置缓动函数，可以根据需要调整
                        .SetUpdate(UpdateType.Normal); // 使用常规更新
                }
            }
        }

        public void Initialize(Node10007 node)
        {
            // 初始化音频管理器
            m_AudioManager = transform.GetComponent<AudioManager>();
            this.m_Node10007 = node;
        }
        // 停止旋转动画的方法，在需要时调用
        public void StopRotation()
        {
            if (rotationTween != null && rotationTween.IsActive())
            {
                rotationTween.Kill();
                rotationTween = null;
            }
        }

        private void OnDestroy()
        {
            // 确保在对象销毁时停止旋转动画
            StopRotation();
        }
        /// <summary>
        /// NPC 待机动画
        /// </summary>
        public void PlayIdleAnimation(int type)
        {
            animator?.SetTrigger(type == 1 ? BowIdleHash : IdleHash);
        }
        /// <summary>
        /// NPC 待机低头讲话
        /// </summary>
        public void PlayBowIdleSpeakAnimation()
        {
            animator?.SetTrigger(BowIdleSpeakHash);
        }
        /// <summary>
        /// NPC 待机低头讲话
        /// </summary>
        public void PlayBeattackAnimation()
        {
            // 检查是否处于冷却期
            if (Time.time - lastBeattackTime < BEATTACK_COOLDOWN)
            {
                // 还在冷却期内，忽略此次调用
                return;
            }
            // 更新上次播放时间
            lastBeattackTime = Time.time;
            animator?.SetTrigger(BeattackHash);
        }


        public void OnCollisionEnter(Collision collision)
        {
            PlayBeattackAnimation();
            if (collision.gameObject.CompareTag("PlayerBullet"))
            {
                BeattackCount += 1;
                if (BeattackCount == 1)
                {
                    MessageDispatcher.SendMessageData("10007_JQ1");
                }
                if (BeattackCount == 2)
                {
                    MessageDispatcher.SendMessageData("10007_JQ1_2");
                }
                if (BeattackCount == Step2Index)
                {
                    MessageDispatcher.SendMessageData("10007_JQ2");
                }
                if (BeattackCount == (Step2Index + 1))
                {
                    MessageDispatcher.SendMessageData("10007_JQ2_2");
                }
            }
        }

        /// <summary>
        /// 重置实体状态
        /// </summary>
        public void Reset()
        {
            // 重置被攻击次数
            BeattackCount = 0;

            // 重置上次被击动画时间
            lastBeattackTime = -1f;

            // 重置动画状态
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                PlayIdleAnimation(1);
            }

            // 停止旋转
            StopRotation();
        }

        private void OnDisable()
        {

        }
    }
}