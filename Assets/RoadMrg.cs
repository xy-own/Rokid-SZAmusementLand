/*
* 文件名：RaoudMrg.cs
* 作者：依旧
* Unity版本：2021.3.26f1
* 创建日期：2025/02/14 15:50:53
* 版权：© 2025 DefaultCompany
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQG
{
    /// <summary>
    /// 类：RaoudMrg
    /// 描述：此类的功能和用途...
    /// </summary>
    public class RoadMrg : MonoBehaviour
    {
        public List<Transform> targetPoints = new List<Transform>();
        public Transform player; // 玩家Transform引用
        private int currentIndex = 0; // 当前最近点的索引
        public int visibleRange = 5; // 可见范围（前后各多少个点可见）

        // 添加特效引用
        public GameObject startEffectPrefab; // 起始特效预制体
        public GameObject endEffectPrefab;   // 结束特效预制体
        private GameObject startEffect;       // 起始特效实例
        private GameObject endEffect;         // 结束特效实例

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
            // 初始化所有点为不可见
            SetAllPointsInvisible();
            
            // 初始化特效
            if (startEffectPrefab != null)
            {
                startEffect = Instantiate(startEffectPrefab);
                startEffect.SetActive(false);
                // 初始时可以先不设置父对象，等第一次 Update 时会自动设置
            }
            
            if (endEffectPrefab != null)
            {
                endEffect = Instantiate(endEffectPrefab);
                endEffect.SetActive(false);
                // 初始时可以先不设置父对象，等第一次 Update 时会自动设置
            }
        }

        void Update()
        {
            if (player != null && targetPoints.Count > 0)
            {
                UpdateNearestPointIndex();
                UpdateVisiblePoints();
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

        // 更新最近点索引
        private void UpdateNearestPointIndex()
        {
            float minDistance = float.MaxValue;
            int newIndex = currentIndex;

            // 搜索范围限制，避免每次都遍历整个列表
            int searchStart = Mathf.Max(0, currentIndex - 10);
            int searchEnd = Mathf.Min(targetPoints.Count, currentIndex + 10);

            for (int i = searchStart; i < searchEnd; i++)
            {
                if (targetPoints[i] == null) continue;

                float distance = Vector3.Distance(player.position, targetPoints[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    newIndex = i;
                }
            }

            currentIndex = newIndex;
        }

        // 更新可见点和特效
        private void UpdateVisiblePoints()
        {
            // 计算可见范围
            int startIndex = Mathf.Max(0, currentIndex - visibleRange);
            int endIndex = Mathf.Min(targetPoints.Count - 1, currentIndex + visibleRange);

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
            UpdateEffects(startIndex, endIndex);
        }

        // 更新特效位置和显示
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

        void OnDestroy()
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