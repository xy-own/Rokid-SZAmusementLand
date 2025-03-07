using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

namespace SU10007
{
    /// <summary>
    /// 生命值组件 - 处理实体的生命值和伤害
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Header("生命值设置")]
        [Tooltip("最大生命值")]
        public float maxHealth = 100f;               // 最大生命值
        [Tooltip("当前生命值")]
        public float currentHealth = 100f;           // 当前生命值
        [Tooltip("无敌时间(秒)")]
        public float invincibleDuration = 0.5f;      // 受伤后的无敌时间

        [Header("显示设置")]
        [Tooltip("是否显示伤害数字")]
        public bool showDamageNumbers = true;        // 是否显示伤害数字
        [Tooltip("伤害数字预制体")]
        public GameObject damageNumberPrefab;        // 伤害数字预制体
        [Tooltip("伤害数字显示高度偏移")]
        public float damageNumberYOffset = 1.5f;     // 伤害数字显示高度偏移
        [Tooltip("伤害数字显示时间(秒)")]
        public float damageNumberDuration = 1.0f;    // 伤害数字显示时间

        [Header("死亡设置")]
        [Tooltip("死亡时是否销毁游戏对象")]
        public bool destroyOnDeath = false;          // 死亡时是否销毁
        [Tooltip("销毁延迟时间(秒)")]
        public float destroyDelay = 1.5f;            // 销毁延迟时间
        [Tooltip("死亡特效预制体")]
        public GameObject deathEffectPrefab;         // 死亡特效预制体

        [Header("事件")]
        public UnityEvent<float, string> OnDamaged;  // 受伤事件
        public UnityEvent OnDeath;                   // 死亡事件
        public UnityEvent OnHealthChanged;           // 生命值变化事件

        private float lastDamageTime;                // 上次受伤时间
        private bool isDead = false;                 // 是否已死亡

        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            // 确保当前生命值不超过最大值
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害量</param>
        /// <param name="damageSource">伤害来源</param>
        /// <returns>是否成功造成伤害</returns>
        public bool TakeDamage(float damage, string damageSource = "")
        {
            // 检查是否处于无敌状态
            if (Time.time - lastDamageTime < invincibleDuration)
            {
                return false;
            }

            // 检查是否已经死亡
            if (isDead || currentHealth <= 0)
            {
                return false;
            }

            // 更新受伤时间
            lastDamageTime = Time.time;

            // 计算实际伤害
            currentHealth -= damage;
            OnHealthChanged?.Invoke();

            // 触发伤害事件
            OnDamaged?.Invoke(damage, damageSource);

            // 显示伤害数字
            if (showDamageNumbers)
            {
                ShowDamageNumber(damage);
            }

            // 检查是否死亡
            if (currentHealth <= 0)
            {
                Die();
            }

            return true;
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        /// <param name="amount">恢复量</param>
        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke();
        }

        /// <summary>
        /// 处理死亡
        /// </summary>
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            currentHealth = 0;

            // 播放死亡特效
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // 触发死亡事件
            OnDeath?.Invoke();

            // 如果设置了死亡销毁，则延迟销毁对象
            if (destroyOnDeath)
            {
                Destroy(gameObject, destroyDelay);
            }

            Debug.Log($"{gameObject.name} 已死亡");
        }

        /// <summary>
        /// 显示伤害数字
        /// </summary>
        /// <param name="damage">伤害量</param>
        private void ShowDamageNumber(float damage)
        {
            // 如果没有预制体，则使用Debug.Log显示伤害
            if (damageNumberPrefab == null)
            {
                Debug.Log($"对 {gameObject.name} 造成 {damage} 点伤害");
                return;
            }

            // 计算伤害数字显示位置
            Vector3 damagePosition = transform.position + Vector3.up * damageNumberYOffset;

            // 随机轻微偏移，避免叠加
            damagePosition.x += UnityEngine.Random.Range(-0.5f, 0.5f);
            damagePosition.z += UnityEngine.Random.Range(-0.5f, 0.5f);

            // 创建伤害数字
            GameObject damageNumberObject = Instantiate(damageNumberPrefab, damagePosition, Quaternion.identity);

            // 设置伤害数值文本
            TextMeshPro textMesh = damageNumberObject.GetComponentInChildren<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damage.ToString("0");

                // 根据伤害大小调整颜色
                if (damage >= 30f)
                {
                    textMesh.color = Color.red;  // 高伤害为红色
                }
                else if (damage >= 15f)
                {
                    textMesh.color = Color.yellow;  // 中伤害为黄色
                }
                else
                {
                    textMesh.color = Color.white;  // 低伤害为白色
                }
            }

            // 添加上升和淡出效果
            StartCoroutine(AnimateDamageNumber(damageNumberObject));
        }

        /// <summary>
        /// 伤害数字动画协程
        /// </summary>
        /// <param name="damageObj">伤害数字对象</param>
        private IEnumerator AnimateDamageNumber(GameObject damageObj)
        {
            if (damageObj == null) yield break;

            float elapsed = 0f;
            Vector3 startPos = damageObj.transform.position;
            Vector3 endPos = startPos + Vector3.up * 1.0f;
            TextMeshPro textMesh = damageObj.GetComponentInChildren<TextMeshPro>();

            // 逐渐上升和淡出
            while (elapsed < damageNumberDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / damageNumberDuration;

                // 移动
                damageObj.transform.position = Vector3.Lerp(startPos, endPos, t);

                // 淡出
                if (textMesh != null)
                {
                    Color color = textMesh.color;
                    color.a = Mathf.Lerp(1f, 0f, t);
                    textMesh.color = color;
                }

                yield return null;
            }

            // 动画结束，销毁对象
            Destroy(damageObj);
        }

        /// <summary>
        /// 重置生命值
        /// </summary>
        public void ResetHealth()
        {
            isDead = false;
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke();
        }

        /// <summary>
        /// 获取生命值百分比
        /// </summary>
        /// <returns>当前生命值百分比(0-1)</returns>
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0;
        }
    }
}
