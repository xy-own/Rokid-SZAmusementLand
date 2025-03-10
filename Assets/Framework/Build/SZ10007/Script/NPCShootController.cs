using System.Collections;
using UnityEngine;

namespace SU10007
{
    /// <summary>
    /// NPC射击控制器 - 控制NPC的射击行为
    /// </summary>
    public class NPCShootController : MonoBehaviour
    {
        [Header("射击设置")]
        [Tooltip("是否启用射击")]
        public bool enableShooting = false;            // 是否启用射击
        [Tooltip("射击目标")]
        public Transform shootTarget;                  // 射击目标
        [Tooltip("射击点")]
        public Transform firePoint;                    // 射击点
        [Tooltip("最小射击间隔(秒)")]
        public float ShootS1Interval = 0.5f;          // 最小射击间隔
        [Tooltip("最大射击间隔(秒)")]
        public float ShootS2Interval = 1.5f;          // 最大射击间隔
        [Tooltip("射击音效名称")]
        public string shootSoundName = "";    // 射击音效名称

        [Header("自动射击设置")]
        private ShootingSystem shootingSystem;         // 射击系统引用
        private Coroutine shootingCoroutine;           // 射击协程

        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            // 如果自动射击被设置为启用，则在开始时启动自动射击
            // if (enableAutoShooting)
            // {
            //     EnableAutoShooting();
            // }
        }

        /// <summary>
        /// 设置射击系统引用
        /// </summary>
        public void SetShootingSystem(ShootingSystem system, GameObject bulletPrefab)
        {
            shootingSystem = system;
            shootingSystem.npcBulletPrefab = bulletPrefab;
        }

        /// <summary>
        /// 启用射击
        /// </summary>
        /// <param name="target">射击目标</param>
        public void EnableShooting(Transform target = null)
        {
            if (target != null)
            {
                shootTarget = target;
            }

            if (shootTarget == null)
            {
                Debug.LogWarning("未设置射击目标，无法启用射击");
                return;
            }

            enableShooting = true;

            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(ShootRoutine());
            }
        }

        /// <summary>
        /// 禁用射击
        /// </summary>
        public void DisableShooting()
        {
            enableShooting = false;

            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }


        /// <summary>
        /// 射击协程
        /// </summary>
        private IEnumerator ShootRoutine()
        {
            while (enableShooting && shootingSystem != null)
            {
                if (enableShooting && shootTarget != null)
                {
                    ShootAnimationRoutine();
                    yield return new WaitForSeconds(ShootS1Interval);
                    Shoot();
                }
                yield return new WaitForSeconds(ShootS2Interval);
            }
        }

        /// <summary>
        /// 执行射击
        /// </summary>
        public void Shoot()
        {
            if (shootingSystem == null)
            {
                Debug.LogWarning("射击系统未设置，无法射击");
                return;
            }

            if (shootTarget == null)
            {
                Debug.LogWarning("未设置射击目标，无法射击");
                return;
            }

            // 确定射击位置
            Vector3 shootPosition = firePoint != null ? firePoint.position : transform.position;

            // 执行射击
            bool shotFired = shootingSystem.NPCShootAtTarget(
                firePoint != null ? firePoint : transform,
                shootTarget.gameObject,
                shootSoundName
            );

            // 显示调试射线
            // if (shotFired && showDebugRay)
            // {
            //     Debug.DrawLine(shootPosition, shootTarget.position, debugRayColor, debugRayDuration);
            // }
        }

        /// <summary>
        /// 设置射击目标
        /// </summary>
        /// <param name="target">目标Transform</param>
        public void SetTarget(Transform target)
        {
            if (target != null)
            {
                shootTarget = target;

                // 如果已经启用了射击，但没有协程在运行，则启动协程
                if (enableShooting && shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(ShootRoutine());
                }
            }
        }

        /// <summary>
        /// 添加射击动画处理
        /// </summary>
        /// <returns>等待协程</returns>
        private void ShootAnimationRoutine()
        {
            // 这里可以添加射击动画逻辑，例如播放射击动画
            Animator animator = transform.GetChild(0).GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // 等待动画完成
            // yield return new WaitForSeconds(0.2f);
        }

        /// <summary>
        /// Unity OnEnable 回调
        /// </summary>
        private void OnEnable()
        {
            // 如果启用射击且没有协程在运行，则启动协程
            if (enableShooting && shootingCoroutine == null && shootTarget != null)
            {
                shootingCoroutine = StartCoroutine(ShootRoutine());
            }
        }

        /// <summary>
        /// Unity OnDisable 回调
        /// </summary>
        private void OnDisable()
        {
            // 停止射击协程
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }

        /// <summary>
        /// 检查组件完整性
        /// </summary>
        private void OnValidate()
        {
            // if (firePoint == null)
            // {
            //     // 尝试查找或创建射击点
            //     Transform existingFirePoint = transform.Find("FirePoint");
            //     if (existingFirePoint == null)
            //     {
            //         GameObject newFirePoint = new GameObject("FirePoint");
            //         newFirePoint.transform.parent = transform;
            //         newFirePoint.transform.localPosition = new Vector3(0, 1.5f, 0.3f);
            //         firePoint = newFirePoint.transform;
            //     }
            //     else
            //     {
            //         firePoint = existingFirePoint;
            //     }
            // }

            // 确保最小间隔不大于最大间隔
            // if (minShootInterval > maxShootInterval)
            // {
            //     maxShootInterval = minShootInterval;
            // }
        }
    }
}
