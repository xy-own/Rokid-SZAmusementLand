/*
* 文件名：#SCRIPTFULLNAME#
* 作者：#AUTHOR#
* Unity版本：#UNITYVERSION#
* 创建日期：#DATE#
* 版权：© #YEAR# #COMPANY#
* All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture;

namespace SU10007
{
    /// <summary>
    /// 类：Node10007
    /// 描述：此类的功能和用途...
    /// </summary>
    public class Node10007 : MonoBehaviour
    {
        public Transform cube;
        public Transform MaoMaoChong;
        public GameObject BulletPrefab; //ProjectileMoveScript

        // 控制发射间隔（秒）
        public float fireInterval = 0.5f;
        private float lastFireTime = 0f;
        // 当手势发射方向与目标方向夹角小于此阈值时自动锁定目标（单位：度）
        public float targetAimAngleThreshold = 10f;

        //Awake
        void Awake()
        {
            MessageDispatcher.AddListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent, PalmEvent);
        }

        private void PalmEvent(PalmEvent palmEvent)
        {
            // 当手势状态有效并满足发射间隔条件时发射子弹
            if (palmEvent.status && Time.time - lastFireTime >= fireInterval)
            {
                ShootBullet(palmEvent);
                lastFireTime = Time.time;
            }
        }

        // 修改发射子弹逻辑，根据手势发射方向与目标方向的角度差，判断是否锁定目标
        private void ShootBullet(PalmEvent palmEvent)
        {
            if (BulletPrefab == null) return;

            // 调试输出手势返回的位置数据
            Debug.Log("Received PalmEvent position: " + palmEvent.position);

            // 若手势数据为屏幕坐标，可使用以下转换（此行为可选，视具体数据而定）：
            // Vector3 worldPos = Camera.main.ScreenToWorldPoint(palmEvent.position);
            // worldPos.z = palmEvent.position.z;
            // Vector3 spawnPosition = worldPos;

            // 如果手势数据已经是世界坐标，则直接使用：
            Vector3 spawnPosition = palmEvent.position;
            Quaternion spawnRotation = Quaternion.Euler(palmEvent.eulerAngles);

            // 如果目标物体存在，则判断发射方向与目标方向的角度差
            if (MaoMaoChong != null)
            {
                Vector3 defaultForward = spawnRotation * Vector3.forward;
                Vector3 targetDirection = (MaoMaoChong.position - spawnPosition).normalized;
                float angle = Vector3.Angle(defaultForward, targetDirection);
                Debug.Log("Angle to target: " + angle);
                if (angle <= targetAimAngleThreshold)
                {
                    spawnRotation = Quaternion.LookRotation(targetDirection);
                }
            }

            // 确保 BulletPrefab 的 pivot 在中心，否则也会出现偏差
            ProjectileMoveScript projectile = Instantiate(BulletPrefab, spawnPosition, spawnRotation).GetComponent<ProjectileMoveScript>();
            // projectile.SetTarget(MaoMaoChong.gameObject, null);
        }

        //Start 回收内存
        void Start()
        {

        }
        //待办 初始化
        void Initialize()
        {

        }
        //OnDestroy 回收内存
        void OnDestroy()
        {

        }
    }
}