/*
* 文件名：RoadMrg.cs
* 作者：依旧
* Unity版本：2021.3.26f1
* 创建日期：2025/02/14 15:50:53
* 描述：路径管理器，用于控制路径点的显示和特效
* 功能：
* 1. 动态显示/隐藏路径点
* 2. 跟踪玩家位置更新最近路径点
* 3. 管理路径起始和结束特效
* 4. 优化性能的动态搜索范围
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQG
{
    /// <summary>
    /// 路径管理器
    /// 负责管理路径点的显示、隐藏以及相关特效
    /// </summary>
    public class RoadMrg : MonoBehaviour
    {
        [Header("基础设置")]
        [Tooltip("路径点列表")]
        public List<Transform> targetPoints = new List<Transform>();
        [Tooltip("玩家Transform引用")]
        public Transform player;
        private int currentIndex = 0; // 当前最近点的索引

        [Header("可见范围设置")]
        [Tooltip("前方可见路径点数量")]
        public int forwardVisibleRange = 5;  // 前方显示的路径点数量
        [Tooltip("后方可见路径点数量")]
        public int backwardVisibleRange = 3; // 后方显示的路径点数量

        [Header("特效设置")]
        [Tooltip("起始点特效预制体")]
        public GameObject startEffectPrefab;  // 起始特效预制体
        [Tooltip("结束点特效预制体")]
        public GameObject endEffectPrefab;    // 结束特效预制体
        private GameObject startEffect;        // 起始特效实例
        private GameObject endEffect;          // 结束特效实例

        [Header("性能优化参数")]
        [Tooltip("更新检测的最小间隔时间(秒)")]
        public float updateInterval = 0.1f;    // 更新间隔时间
        [Tooltip("动态搜索范围")]
        public int searchRange = 10;           // 搜索范围大小
        [Tooltip("触发重新搜索的距离阈值")]
        public float researchThreshold = 5f;   // 重新搜索的距离阈值
        [Tooltip("触发全局搜索的距离阈值")]
        public float globalSearchThreshold = 10f; // 触发全局搜索的距离阈值

        // 性能优化相关的私有变量
        private float lastUpdateTime;          // 上次更新时间
        private Vector3 lastPlayerPos;         // 上次玩家位置
        private float sqrResearchThreshold;    // 距离阈值的平方
        private float sqrGlobalSearchThreshold; // 全局搜索阈值的平方

        void Awake()
        {
            // 确保有玩家引用
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
        }

        void Start()
        {
            if (player == null || targetPoints.Count == 0) return;

            // 先找到最近的点作为起始点
            FindInitialNearestPoint();

            // 初始化所有点为不可见
            SetAllPointsInvisible();

            // 初始化特效
            InitializeEffects();

            lastPlayerPos = player.position;
            sqrResearchThreshold = researchThreshold * researchThreshold;
            sqrGlobalSearchThreshold = globalSearchThreshold * globalSearchThreshold;

            // 立即更新可见点
            UpdateVisiblePoints();
        }

        /// <summary>
        /// 查找初始最近点
        /// </summary>
        private void FindInitialNearestPoint()
        {
            float minSqrDistance = float.MaxValue;
            Vector3 playerPos = player.position;

            // 遍历所有点找到最近的
            for (int i = 0; i < targetPoints.Count; i++)
            {
                if (targetPoints[i] == null) continue;

                float sqrDistance = (playerPos - targetPoints[i].position).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    currentIndex = i;
                }
            }
        }

        /// <summary>
        /// 初始化特效
        /// </summary>
        private void InitializeEffects()
        {
            if (startEffectPrefab != null)
            {
                startEffect = Instantiate(startEffectPrefab);
                startEffect.SetActive(false);
            }

            if (endEffectPrefab != null)
            {
                endEffect = Instantiate(endEffectPrefab);
                endEffect.SetActive(false);
            }
        }

        void Update()
        {
            if (player == null || targetPoints.Count == 0) return;

            // 检查是否需要更新
            if (ShouldUpdateSearch())
            {
                // 检查是否需要全局搜索（玩家移动距离过大）
                if (NeedsGlobalSearch())
                {
                    FindInitialNearestPoint(); // 执行全局搜索
                }
                else
                {
                    UpdateNearestPointIndex();
                }

                UpdateVisiblePoints();
                lastUpdateTime = Time.time;
                lastPlayerPos = player.position;
            }
        }

        // 设置所有点不可见
        private void SetAllPointsInvisible()
        {
            foreach (Transform point in targetPoints)
            {
                if (point != null)
                    point.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新最近点索引
        /// 使用动态搜索范围优化性能
        /// </summary>
        private void UpdateNearestPointIndex()
        {
            float minSqrDistance = float.MaxValue;
            int newIndex = currentIndex;
            Vector3 playerPos = player.position;

            // 计算当前索引点与玩家的距离
            if (currentIndex >= 0 && currentIndex < targetPoints.Count && targetPoints[currentIndex] != null)
            {
                float currentSqrDistance = (playerPos - targetPoints[currentIndex].position).sqrMagnitude;
                // 如果距离太大，触发全局搜索
                if (currentSqrDistance > sqrGlobalSearchThreshold)
                {
                    FindInitialNearestPoint();
                    return;
                }
            }

            // 动态调整搜索范围
            int searchStart = Mathf.Max(0, currentIndex - searchRange);
            int searchEnd = Mathf.Min(targetPoints.Count, currentIndex + searchRange);

            // 使用距离平方进行比较
            for (int i = searchStart; i < searchEnd; i++)
            {
                if (targetPoints[i] == null) continue;

                float sqrDistance = (playerPos - targetPoints[i].position).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    newIndex = i;
                }
            }

            // 如果找到更近的点，动态调整搜索范围
            if (newIndex != currentIndex)
            {
                currentIndex = newIndex;
                // 根据距离差异动态调整搜索范围
                searchRange = Mathf.Max(5, Mathf.Min(20, Mathf.Abs(newIndex - currentIndex) * 2));
            }
        }

        /// <summary>
        /// 更新可见点和特效
        /// 根据当前索引更新路径点的显示状态
        /// </summary>
        private void UpdateVisiblePoints()
        {
            // 计算前后可见范围
            int startIndex = Mathf.Max(0, currentIndex - backwardVisibleRange);
            int endIndex = Mathf.Min(targetPoints.Count - 1, currentIndex + forwardVisibleRange);

            // 设置范围内的点可见，范围外的点不可见
            for (int i = 0; i < targetPoints.Count; i++)
            {
                if (targetPoints[i] == null) continue;

                bool shouldBeVisible = (i >= startIndex && i <= endIndex);
                if (targetPoints[i].gameObject.activeSelf != shouldBeVisible)
                {
                    targetPoints[i].gameObject.SetActive(shouldBeVisible);
                }
            }

            // 更新特效位置
            // UpdateEffects(startIndex, endIndex);
        }

        /// <summary>
        /// 更新特效位置和显示
        /// 将特效附加到对应的路径点上
        /// </summary>
        /// <param name="startIndex">起始路径点索引</param>
        /// <param name="endIndex">结束路径点索引</param>
        private void UpdateEffects(int startIndex, int endIndex)
        {
            if (targetPoints.Count == 0) return;

            // 更新起始特效
            if (startEffect != null && startIndex >= 0 && startIndex < targetPoints.Count)
            {
                startEffect.SetActive(true);
                // 将特效设置为 Point_D (后面点) 的子对象
                Transform pointD = targetPoints[startIndex].Find("Point_D");
                if (pointD != null)
                {
                    startEffect.transform.SetParent(pointD, false);
                    startEffect.transform.localPosition = Vector3.zero;
                }
            }

            // 更新结束特效
            if (endEffect != null && endIndex >= 0 && endIndex < targetPoints.Count)
            {
                endEffect.SetActive(true);
                // 将特效设置为 Point_F (前面点) 的子对象
                Transform pointF = targetPoints[endIndex].Find("Point_F");
                if (pointF != null)
                {
                    endEffect.transform.SetParent(pointF, false);
                    endEffect.transform.localPosition = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// 检查是否需要更新搜索
        /// 基于时间间隔和距离阈值进行判断
        /// </summary>
        /// <returns>是否需要更新</returns>
        private bool ShouldUpdateSearch()
        {
            // 检查更新间隔
            if (Time.time - lastUpdateTime < updateInterval) return false;

            // 检查移动距离是否超过阈值
            float sqrDistance = (player.position - lastPlayerPos).sqrMagnitude;
            return sqrDistance > sqrResearchThreshold;
        }

        /// <summary>
        /// 检查是否需要进行全局搜索
        /// 当玩家移动距离特别大时触发（如传送）
        /// </summary>
        /// <returns>是否需要全局搜索</returns>
        private bool NeedsGlobalSearch()
        {
            float sqrDistance = (player.position - lastPlayerPos).sqrMagnitude;
            return sqrDistance > sqrGlobalSearchThreshold;
        }

        /// <summary>
        /// 资源清理
        /// </summary>
        private void OnDestroy()
        {
            targetPoints.Clear();

            // 清理特效
            if (startEffect != null)
                Destroy(startEffect);
            if (endEffect != null)
                Destroy(endEffect);
        }
    }
}