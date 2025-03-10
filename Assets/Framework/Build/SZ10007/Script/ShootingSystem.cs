using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SU10007
{
    /// <summary>
    /// 射击系统 - 管理子弹发射、目标锁定等功能
    /// </summary>
    public class ShootingSystem : MonoBehaviour
    {
        [Header("子弹设置")]
        public GameObject playerBulletPrefab;          // 玩家子弹预制体
        public GameObject npcBulletPrefab;             // NPC子弹预制体

        [Header("射击参数")]
        [Tooltip("发射间隔时间(秒)")]
        public float fireInterval = 0.5f;              // 发射间隔时间
        [Tooltip("子弹飞行速度")]
        public float bulletSpeed = 30f;                // 子弹速度
        [Tooltip("当手势方向与目标夹角小于此值时自动锁定（度）")]
        public float targetAimAngleThreshold = 80f;    // 自动锁定目标角度阈值
        [Tooltip("子弹精确度(0-100%)")]
        [Range(0, 100)]
        public float accuracy = 100f;                   // 子弹精确度
        [Tooltip("子弹存活时间(秒)")]
        public float bulletLifetime = 2f;              // 子弹存活时间
        [Tooltip("锁定目标时的随机角度偏移最大值（度）")]
        public float targetRandomOffset = 5f;          // 锁定目标时的随机角度偏移

        [Header("NPC射击设置")]
        [Tooltip("NPC射击精度(0-100%)")]
        [Range(0, 100)]
        public float npcAccuracy = 85f;                // NPC射击精度
        [Tooltip("NPC射击间隔(秒)")]
        public float npcFireInterval = 1.5f;           // NPC射击间隔
        [Tooltip("NPC子弹速度")]
        public float npcBulletSpeed = 12f;             // NPC子弹速度
        [Tooltip("NPC锁定目标时的随机角度偏移最大值（度）")]
        public float npcTargetRandomOffset = 8f;       // NPC锁定目标时的随机角度偏移

        private AudioManager audioManager;             // 音频管理器引用
        private float lastPlayerFireTime = -10f;       // 玩家上次发射时间
        private float lastNpcFireTime = -10f;          // NPC上次发射时间

        // 射手类型枚举
        public enum ShooterType
        {
            Player,
            NPC
        }

        /// <summary>
        /// 初始化射击系统
        /// </summary>
        /// <param name="audioManager">音频管理器实例</param>
        public void Initialize(AudioManager audioManager)
        {
            this.audioManager = audioManager;
            lastPlayerFireTime = -fireInterval; // 设置为负值以便可以立即发射第一发子弹
            lastNpcFireTime = -npcFireInterval;

            // if (playerBulletPrefab == null)
            // {
            //     Debug.LogWarning("玩家子弹预制体未设置");
            // }

            // if (npcBulletPrefab == null)
            // {
            //     Debug.LogWarning("NPC子弹预制体未设置，将使用玩家子弹预制体");
            //     npcBulletPrefab = playerBulletPrefab;
            // }
        }

        /// <summary>
        /// 发射子弹
        /// </summary>
        /// <param name="position">发射位置</param>
        /// <param name="rotation">发射方向</param>
        /// <param name="target">可选的目标对象</param>
        /// <param name="soundName">发射音效名称</param>
        /// <param name="shooterType">射手类型</param>
        /// <returns>是否成功发射</returns>
        public bool Shoot(Vector3 position, Quaternion rotation, GameObject target = null,
                          string soundName = null, ShooterType shooterType = ShooterType.Player)
        {
            // 根据射手类型选择相应的参数
            float currentInterval = shooterType == ShooterType.Player ? fireInterval : npcFireInterval;
            float currentSpeed = shooterType == ShooterType.Player ? bulletSpeed : npcBulletSpeed;
            float currentAccuracy = shooterType == ShooterType.Player ? accuracy : npcAccuracy;
            GameObject currentBulletPrefab = shooterType == ShooterType.Player ? playerBulletPrefab : npcBulletPrefab;
            ref float lastFireTime = ref shooterType == ShooterType.Player ? ref lastPlayerFireTime : ref lastNpcFireTime;
            float randomOffsetMax = shooterType == ShooterType.Player ? targetRandomOffset : npcTargetRandomOffset;

            // 检查发射冷却时间
            if (Time.time - lastFireTime < currentInterval)
            {
                return false;
            }

            // 更新发射时间
            lastFireTime = Time.time;

            // 检查子弹预制体
            if (currentBulletPrefab == null)
            {
                Debug.LogWarning($"{shooterType} 子弹预制体未设置，无法发射");
                return false;
            }

            // 播放音效
            if (audioManager != null && !string.IsNullOrEmpty(soundName))
            {
                audioManager.PlaySound(soundName);
            }

            // 计算发射位置（在发射点前方一小段距离）
            Vector3 spawnPosition = position + rotation * Vector3.forward * 0.1f;

            // 目标锁定逻辑
            if (target != null)
            {
                // 计算方向与目标方向的夹角
                Vector3 targetDirection = (target.transform.position - spawnPosition).normalized;
                float angle = Vector3.Angle(rotation * Vector3.forward, targetDirection);
                // Debug.Log(angle);
                // 如果夹角小于阈值，则锁定目标（并添加随机偏移）
                if (angle < targetAimAngleThreshold)
                {
                    // 先计算基础的目标朝向
                    rotation = Quaternion.LookRotation(targetDirection);

                    // 添加随机偏移，使子弹不会总是打在同一个点上
                    // 在水平和垂直方向上各添加一个随机角度
                    float randomYaw = UnityEngine.Random.Range(-randomOffsetMax, randomOffsetMax);
                    float randomPitch = UnityEngine.Random.Range(-randomOffsetMax, randomOffsetMax);
                    rotation = rotation * Quaternion.Euler(randomPitch, randomYaw, 0);

                    if (shooterType == ShooterType.Player)
                    {
                        Debug.Log($"玩家锁定目标，夹角: {angle}度，添加随机偏移: X={randomPitch}, Y={randomYaw}");
                    }
                }
            }

            // 实例化子弹
            GameObject bullet = Instantiate(currentBulletPrefab, spawnPosition, rotation);

            // 设置子弹参数
            ProjectileMoveScript projectileScript = bullet.GetComponent<ProjectileMoveScript>();
            if (projectileScript != null)
            {
                projectileScript.speed = currentSpeed;
                projectileScript.accuracy = currentAccuracy;

                // 如果有目标且已锁定，则设置目标
                if (target != null && Vector3.Angle(rotation * Vector3.forward,
                    (target.transform.position - spawnPosition).normalized) < targetAimAngleThreshold)
                {
                    projectileScript.SetTarget(target);
                }

                // 设置子弹标签以区分来源
                bullet.tag = shooterType == ShooterType.Player ? "PlayerBullet" : "NPCBullet";

                // 添加子弹自毁逻辑（超时自毁）
                Destroy(bullet, bulletLifetime);
            }
            else
            {
                Debug.LogWarning("子弹预制体上没有ProjectileMoveScript组件");
                Destroy(bullet);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 让NPC向目标射击
        /// </summary>
        /// <param name="npcTransform">NPC的Transform</param>
        /// <param name="target">目标GameObject</param>
        /// <param name="soundName">射击音效名称</param>
        /// <returns>是否成功射击</returns>
        public bool NPCShootAtTarget(Transform npcTransform, GameObject target, string soundName)
        {
            if (target == null || npcTransform == null)
            {
                return false;
            }

            // 计算从NPC到目标的方向
            Vector3 direction = (target.transform.position - npcTransform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 从NPC位置发射子弹
            Vector3 firePosition = npcTransform.position; // 假设发射点在NPC头部

            return Shoot(firePosition, rotation, target, soundName, ShooterType.NPC);
        }

        /// <summary>
        /// 设置子弹精确度
        /// </summary>
        /// <param name="newAccuracy">新的精确度值(0-100)</param>
        /// <param name="shooterType">射手类型</param>
        public void SetAccuracy(float newAccuracy, ShooterType shooterType = ShooterType.Player)
        {
            if (shooterType == ShooterType.Player)
            {
                accuracy = Mathf.Clamp(newAccuracy, 0f, 100f);
            }
            else
            {
                npcAccuracy = Mathf.Clamp(newAccuracy, 0f, 100f);
            }
        }

        /// <summary>
        /// 设置发射间隔
        /// </summary>
        /// <param name="interval">间隔时间(秒)</param>
        /// <param name="shooterType">射手类型</param>
        public void SetFireInterval(float interval, ShooterType shooterType = ShooterType.Player)
        {
            if (shooterType == ShooterType.Player)
            {
                fireInterval = Mathf.Max(0.1f, interval);
            }
            else
            {
                npcFireInterval = Mathf.Max(0.1f, interval);
            }
        }
    }
}
