/*
* 文件名：Node10007.cs
* 作者：优化版本
* Unity版本：2022.3 LTS
* 创建日期：2023-10-01
* 版权：© 2023 Rokid
* All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using XY.UXR.Gesture;

namespace SU10007
{
    /// <summary>
    /// 类：Node10007
    /// 描述：手势控制子弹发射系统，实现根据手势方向发射子弹并可选择性地锁定目标
    /// </summary>
    public class Node10007 : MonoBehaviour
    {
        [Header("目标与射击设置")]
        public GameObject BulletPrefab;                 // 子弹预制体
        public GameObject SDBulletPrefab;                 // 子弹预制体
        public GameObject JBRBulletPrefab;                 // 子弹预制体

        [Header("射击参数")]
        [Tooltip("控制发射间隔（秒）")]
        public float fireInterval = 0.5f;               // 发射间隔时间
        [Tooltip("当手势方向与目标夹角小于此值时自动锁定（度）")]
        public float targetAimAngleThreshold = 10f;     // 自动锁定目标的角度阈值

        [Header("NPC射击设置")]
        // [Tooltip("NPC子弹预制体")]
        // public GameObject NPCBulletPrefab;            // NPC子弹预制体
        [Tooltip("NPC是否可以射击")]
        public bool enableNpcShooting = true;         // NPC是否可以射击
        [Tooltip("NPC射击间隔(秒)")]
        public float npcFireInterval = 2f;            // NPC射击间隔

        [Header("调试设置")]
        [Tooltip("启用测试GUI按钮")]
        public bool enableTestGUI = true;               // 是否启用测试GUI
        [Tooltip("GUI按钮位置水平偏移")]
        public float testGUIOffsetX = 10f;              // GUI按钮水平位置
        [Tooltip("GUI按钮位置垂直偏移")]
        public float testGUIOffsetY = 10f;              // GUI按钮垂直位置

        [Header("射击控制")]
        [Tooltip("是否已经允许玩家射击")]
        public bool canShoot = false;                  // 控制玩家是否可以射击
        [Tooltip("是否在介绍完成后自动启用射击")]
        public bool autoEnableShootingAfterIntro = true; // 是否在介绍完成后自动启用射击

        private Transform CameraPos;
        private CancellationTokenSource cancellationToken; // 取消令牌
        private bool playerInTriggerZone = false;       // 玩家是否在触发区域内
        private GameObject m_Audio;                     // 音频对象
        private AudioManager m_AudioManager;            // 音频管理器
        private GameObject m_Enter;                     // 进入触发区对象
        private GameObject m_Exit;                      // 退出触发区对象
        private GameObject m_SceneModel;                // 场景模型
        private GameObject m_SceneEffect;               // 场景特效
        private GameObject m_LiHeModel;                 // 礼盒模型
        private SuNiEntity m_SuNiEntity;                // SuNi实体
        private SuDiEntity m_SuDiEntity;                // SuDi实体
        private JBREntity m_JBREntity;                 // 姜饼人实体
        private MMCEntity m_MMCEntity;                  // 毛毛虫实体
        private Transform m_MMCP1;                      // 毛毛虫出现点1
        private Transform m_MMCP2;                      // 毛毛虫出现点2
        private ShootingSystem shootingSystem;          // 射击系统
        private NPCShootController npcSDShootController;  // SuDI射击控制器
        private ShootingSystem shootingSystemSD;          // SuDI射击系统
        private NPCShootController npcJBRShootController;  // JBR射击控制器
        private ShootingSystem shootingSystemJBR;          // JBR射击系统
        private Camera mainCamera;                      // 主相机引用
        // private Transform playerTarget;                 // 玩家目标点
        private Transform m_ModModel;                   // 柱子模型


        /// <summary>
        /// Awake - 初始化并注册事件监听
        /// </summary>
        void Awake()
        {
            // 注册手势事件监听
            MessageDispatcher.AddListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent, PalmEvent);
            // 查找并初始化组件
            m_Audio = transform.Find("Audio").gameObject;

            CameraPos = Camera.main.transform;
            //显示场景模型
            m_SceneModel = transform.Find("NPC/Scene").gameObject;
            m_LiHeModel = transform.Find("NPC/LiHe").gameObject;
            m_SceneEffect = transform.Find("NPC/SceneEffect").gameObject;
            m_SuDiEntity = transform.Find("NPC/SuDiEntity").GetComponent<SuDiEntity>();
            m_SuNiEntity = transform.Find("NPC/SuNiEntity").GetComponent<SuNiEntity>();
            m_JBREntity = transform.Find("NPC/JBREntity").GetComponent<JBREntity>();
            m_MMCEntity = transform.Find("NPC/MMCEntity").GetComponent<MMCEntity>();
            m_MMCP1 = transform.Find("NPC/MMCP1");
            m_MMCP2 = transform.Find("NPC/MMCP2");
            m_ModModel = transform.Find("NPC/Mod");
            // 设置触发器事件
            m_Enter = transform.Find("Trigger/Enter").gameObject;
            if (m_Enter != null)
            {
                m_Enter.AddComponent<TriEvent>().enterAction += EnterEvent;
            }
            else
            {
                Debug.LogWarning("未找到Enter触发器对象");
            }

            m_Exit = transform.Find("Trigger/Exit").gameObject;
            if (m_Exit != null)
            {
                m_Exit.AddComponent<TriEvent>().exitAction += ExitEvent;
            }
            else
            {
                Debug.LogWarning("未找到Exit触发器对象");
            }

            // 初始化音频管理器
            if (m_Audio != null)
            {
                m_AudioManager = m_Audio.AddComponent<AudioManager>();
            }
            else
            {
                Debug.LogError("音频组件初始化失败");
            }

            // 获取主相机引用
            mainCamera = Camera.main;

            // 初始化射击系统
            shootingSystem = gameObject.AddComponent<ShootingSystem>();
            shootingSystem.playerBulletPrefab = BulletPrefab;
            shootingSystem.fireInterval = fireInterval;
            shootingSystem.targetAimAngleThreshold = targetAimAngleThreshold;
            shootingSystem.Initialize(m_AudioManager);

            m_MMCEntity.Initialize(this);
            // 初始化NPC射击控制器
            InitializeNpcShooting();
        }
        /// <summary>
        /// 初始化系统
        /// </summary>
        void Initialize()
        {
            playerInTriggerZone = false;
            canShoot = false; // 初始状态下禁用射击
            Debug.Log("Node10007系统已初始化");
        }
        /// <summary>
        /// 开始时进行初始化
        /// </summary>
        void Start()
        {
            // 检查必要组件是否已分配
            CheckComponents();

            // 初始化
            Initialize();
        }

        /// <summary>
        /// 检查必要的组件是否已正确设置
        /// </summary>
        private void CheckComponents()
        {
            if (BulletPrefab == null)
            {
                Debug.LogWarning("子弹预制体未设置，请分配BulletPrefab");
            }

            if (m_MMCEntity == null)
            {
                Debug.LogWarning("目标MaoMaoChong未设置，自动锁定功能将不可用");
            }
        }

        /// <summary>
        /// 初始化NPC射击功能
        /// </summary>
        private void InitializeNpcShooting()
        {
            if (!enableNpcShooting) return;

            // 如果毛毛虫实体存在且没有射击控制器，则添加
            if (m_JBREntity != null && m_JBREntity.gameObject.GetComponent<NPCShootController>() == null)
            {
                shootingSystemSD = m_JBREntity.gameObject.AddComponent<ShootingSystem>();
                shootingSystemSD.npcBulletPrefab = JBRBulletPrefab;
                shootingSystemSD.fireInterval = fireInterval;
                shootingSystemSD.targetAimAngleThreshold = targetAimAngleThreshold;
                shootingSystemSD.Initialize(m_AudioManager);

                npcJBRShootController = m_JBREntity.gameObject.AddComponent<NPCShootController>();
                npcJBRShootController.SetShootingSystem(shootingSystemSD, JBRBulletPrefab);
                npcJBRShootController.firePoint = m_JBREntity.transform.GetChild(1);

                // 设置射击参数
                npcJBRShootController.ShootS1Interval = 1f;
                npcJBRShootController.ShootS2Interval = 1f;
                npcJBRShootController.shootSoundName = "npc_shoot";
                npcJBRShootController.enableShooting = false; // 默认不启用射击
            }
            else if (m_JBREntity == null)
            {
                Debug.LogWarning("毛毛虫实体未找到，NPC射击功能将不可用");
            }
            if (m_SuDiEntity != null && m_SuDiEntity.gameObject.GetComponent<NPCShootController>() == null)
            {
                shootingSystemJBR = m_SuDiEntity.gameObject.AddComponent<ShootingSystem>();
                shootingSystemJBR.npcBulletPrefab = SDBulletPrefab;
                shootingSystemJBR.fireInterval = fireInterval;
                shootingSystemJBR.targetAimAngleThreshold = targetAimAngleThreshold;
                shootingSystemJBR.Initialize(m_AudioManager);

                npcSDShootController = m_SuDiEntity.gameObject.AddComponent<NPCShootController>();
                npcSDShootController.SetShootingSystem(shootingSystemJBR, SDBulletPrefab);
                npcSDShootController.firePoint = m_SuDiEntity.transform.GetChild(1);

                // 设置射击参数
                npcSDShootController.ShootS1Interval = 1.2f;
                npcSDShootController.ShootS2Interval = 2.2f;
                npcSDShootController.shootSoundName = "npc_shoot";
                npcSDShootController.enableShooting = false; // 默认不启用射击
            }
            else if (m_SuDiEntity == null)
            {
                Debug.LogWarning("毛毛虫实体未找到，NPC射击功能将不可用");
            }
        }

        /// <summary>
        /// 当玩家退出触发区域时的事件处理
        /// </summary>
        private void ExitEvent()
        {
            MessageDispatcher.SendMessageData("ExitPoi");
            playerInTriggerZone = false;
            DisableShooting(); // 玩家离开区域时禁用射击
            Debug.Log("玩家已离开交互区域");
            SkyboxManager.Instance.SetDefaultSkybox();
            m_ModModel.gameObject.SetActive(true);
            // 可以添加相关的视觉或音频反馈
            if (m_AudioManager != null)
            {
                m_AudioManager.PlaySound("exit");
            }
        }

        /// <summary>
        /// 当玩家进入触发区域时的事件处理
        /// </summary>
        private void EnterEvent()
        {
            MessageDispatcher.SendMessageData("EnterPoi");
            playerInTriggerZone = true;
            Debug.Log("玩家已进入交互区域");
            MessageDispatcher.SendMessageData<string>("SetBgm", "BGM6");
            m_SceneModel.gameObject.SetActive(true);
            m_SceneEffect.gameObject.SetActive(true);
            m_SuNiEntity.gameObject.SetActive(true);
            m_ModModel.gameObject.SetActive(false);
            m_SuNiEntity.LookTarget = m_MMCEntity.transform;
            SkyboxManager.Instance.SetSkybox(SkyboxManager.Instance.targetSkyboxName);
            cancellationToken = new CancellationTokenSource();

            // 在原有逻辑基础上，添加姜饼人和苏迪的出场
            this.DelaySeconds(0.5f, async () =>
            {
                m_MMCEntity.gameObject.SetActive(true);
                m_MMCEntity.transform.position = m_MMCP1.position;
                m_MMCEntity.transform.localScale = m_MMCP1.localScale;
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.Join(m_MMCEntity.transform.DOMove(m_MMCP2.position, 1).SetEase(Ease.OutBounce));
                sequence.Join(m_MMCEntity.transform.DOScale(m_MMCP2.localScale, 1).SetEase(Ease.OutQuad));
                await sequence.AsyncWaitForCompletion();
                m_SceneEffect.gameObject.SetActive(false);
                // 继续原有的对话流程
                this.DelaySeconds(2f, () => // 等待两秒后 SuNI讲话
                {
                    m_SuNiEntity.PlayIdleSpeakAnimation();
                    m_AudioManager.PlaySound("06-1-1", onComplete: () =>
                    {
                        m_SuNiEntity.PlayIdleAnimation();
                        m_MMCEntity.PlayBowIdleSpeakAnimation();
                        m_AudioManager.PlaySound("06-1-2", onComplete: () =>
                        {
                            m_MMCEntity.PlayIdleAnimation(1);
                            m_SuNiEntity.PlayAttackSpeakAnimation();
                            m_SuNiEntity.LookTarget = CameraPos;
                            m_AudioManager.PlaySound("06-2-1", onComplete: () =>
                            {
                                m_SuNiEntity.PlayIdleAnimation();
                                m_SuNiEntity.LookTarget = m_MMCEntity.transform;

                                // 在介绍完成后启用射击功能
                                if (autoEnableShootingAfterIntro)
                                {
                                    EnableShooting();
                                }
                            });
                        });
                    });
                }, cancellationToken.Token).Forget();
            }, cancellationToken.Token).Forget();

            // 可以添加相关的视觉或音频反馈
            if (m_AudioManager != null)
            {
                m_AudioManager.PlaySound("mmc");
            }
        }

        /// <summary>
        /// 让实体从空中跳出来的方法
        /// </summary>
        /// <param name="entity">要跳出的实体对象</param>
        /// <param name="startPosition">起始位置</param>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="duration">动画持续时间</param>
        /// <param name="jumpHeight">跳跃高度</param>
        /// <param name="jumpCount">跳跃次数 (实际跳跃次数)</param>
        /// <param name="soundEffect">播放的音效名称</param>
        /// <returns>异步操作</returns>
        public async UniTask SpawnEntityFromSky(GameObject entity, Vector3 startPosition, Vector3 targetPosition,
            float duration = 1.5f, float jumpHeight = 3f, int jumpCount = 1, string soundEffect = null)
        {
            if (entity == null)
            {
                Debug.LogWarning("实体对象为空，无法执行跳跃动画");
                return;
            }

            // 激活实体并设置初始位置
            entity.SetActive(true);
            entity.transform.position = startPosition;

            // 播放音效
            if (!string.IsNullOrEmpty(soundEffect) && m_AudioManager != null)
            {
                m_AudioManager.PlaySound(soundEffect);
            }

            // 创建动画序列
            Sequence jumpSequence = DOTween.Sequence();

            if (jumpCount <= 0)
            {
                // 如果跳跃次数为0或负数，就直接移动到目标位置
                jumpSequence.Append(entity.transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad));
            }
            else if (jumpCount == 1)
            {
                // 单次跳跃 - 简化路径
                // 计算单次跳跃的顶点
                Vector3 peakPoint = Vector3.Lerp(startPosition, targetPosition, 0.5f);
                peakPoint.y += jumpHeight;

                // 创建三点路径：起点、顶点、终点
                Vector3[] simplePath = new Vector3[3] { startPosition, peakPoint, targetPosition };
                jumpSequence.Append(entity.transform.DOPath(simplePath, duration, PathType.CatmullRom).SetEase(Ease.OutQuad));
            }
            else
            {
                // 多次跳跃逻辑
                // 为每次跳跃分配路径点：起点 + (每次跳跃2个点) + 终点
                Vector3[] path = new Vector3[jumpCount * 2 + 1];
                path[0] = startPosition;
                path[path.Length - 1] = targetPosition;

                for (int i = 0; i < jumpCount; i++)
                {
                    // 计算当前跳跃段的进度
                    float segmentStart = (float)i / jumpCount;
                    float segmentEnd = (float)(i + 1) / jumpCount;

                    if (i == jumpCount - 1)
                    {
                        // 最后一次跳跃直接到达终点
                        segmentEnd = 1.0f;
                    }

                    // 计算当前跳跃的中间点(顶点)
                    Vector3 segmentMid = Vector3.Lerp(startPosition, targetPosition, (segmentStart + segmentEnd) / 2);

                    // 高度随着跳跃次数减少
                    float currentHeight = jumpHeight * (1.0f - (float)i / jumpCount * 0.3f);
                    segmentMid.y += currentHeight;

                    // 保存路径点
                    path[i * 2 + 1] = segmentMid;

                    // 如果不是最后一次跳跃，还需要设置着陆点
                    if (i < jumpCount - 1)
                    {
                        Vector3 landPoint = Vector3.Lerp(startPosition, targetPosition, segmentEnd);
                        path[i * 2 + 2] = landPoint;
                    }
                }

                jumpSequence.Append(entity.transform.DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.OutQuad));
            }

            // 添加缩放动画效果，着陆时压缩再恢复
            Vector3 originalScale = entity.transform.localScale;
            Vector3 compressedScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z * 1.2f);

            jumpSequence.Join(
                DOTween.Sequence()
                    .AppendInterval(duration * 0.8f) // 等待接近着陆
                    .Append(entity.transform.DOScale(compressedScale, duration * 0.1f))
                    .Append(entity.transform.DOScale(originalScale, duration * 0.1f).SetEase(Ease.OutElastic))
            );

            // 等待动画完成
            await jumpSequence.AsyncWaitForCompletion();

            // 可以在这里添加着陆后的额外效果，如灰尘特效等
            Debug.Log($"{entity.name} 已完成跳跃动画，跳跃次数: {jumpCount}");
        }

        /// <summary>
        /// 生成姜饼人和苏迪实体
        /// </summary>
        public async UniTask SpawnJBRAndSuDiEntities()
        {
            // 定义起始和目标位置
            Vector3 jbrStartPos = m_JBREntity.transform.position + new Vector3(0, 5, 0); // 姜饼人从上方15米处出现
            Vector3 jbrTargetPos = m_JBREntity.transform.position;

            Vector3 suDiStartPos = m_SuDiEntity.transform.position + new Vector3(0, 5, 0); // 苏迪从上方12米处出现
            Vector3 suDiTargetPos = m_SuDiEntity.transform.position;

            // 先隐藏实体
            m_JBREntity.gameObject.SetActive(false);
            m_SuDiEntity.gameObject.SetActive(false);

            // 先让姜饼人跳出来 - 修改为跳1次
            SpawnEntityFromSky(m_JBREntity.gameObject, jbrStartPos, jbrTargetPos, 2f, 5f, 1, "jbr_jump").Forget();
            m_JBREntity.LookTarget = m_MMCEntity.transform;
            // 等待0.5秒后让苏迪跳出来
            // await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            await SpawnEntityFromSky(m_SuDiEntity.gameObject, suDiStartPos, suDiTargetPos, 2f, 5f, 1, "sudi_jump");
            m_SuDiEntity.LookTarget = m_MMCEntity.transform;
            m_AudioManager.PlaySound("06-4-1", onComplete: () =>
            {
                m_AudioManager.PlaySound("06-5");
                EnableShooting();
                this.DelaySeconds(0.5f, () =>
                {
                    // 启动NPC射击逻辑
                    if (enableNpcShooting && npcSDShootController != null && npcJBRShootController != null)
                    {
                        // 更新玩家目标位置为相机位置前方
                        // UpdatePlayerTargetPosition();

                        // 启用NPC射击
                        // npcSDShootController.EnableAutoShooting();
                        // npcJBRShootController.EnableAutoShooting();


                        // 设置NPC射击目标为玩家目标点
                        npcSDShootController.EnableShooting(m_MMCEntity.AttackPoint);
                        npcJBRShootController.EnableShooting(m_MMCEntity.AttackPoint);

                        Debug.Log("NPC射击功能已启用，目标：毛毛虫");
                        this.DelaySeconds(5f, () =>
                        {
                            m_MMCEntity.m_AudioManager.PlaySound("06-3-2");
                            this.DelaySeconds(5f, () =>
                            {
                                m_MMCEntity.m_AudioManager.PlaySound("06-3-3", onComplete: () =>
                                {
                                    this.DelaySeconds(5f, () =>
                                    {
                                        m_MMCEntity.m_AudioManager.PlaySound("06-3-4", onComplete: () =>
                                        {
                                            // Debug.Log("NPC射击功能已关闭");
                                            npcSDShootController.DisableShooting();
                                            npcJBRShootController.DisableShooting();
                                            DisableShooting();
                                            // m_MMCEntity.PlayIdleAnimation(1);
                                            m_SceneEffect.gameObject.SetActive(true);
                                            this.DelaySeconds(1f, async () =>
                                            {
                                                // m_MMCEntity.transform.position = m_MMCP1.position;
                                                // m_MMCEntity.transform.localScale = m_MMCP1.localScale;
                                                DG.Tweening.Sequence sequence = DOTween.Sequence();
                                                sequence.Join(m_MMCEntity.transform.DOMove(m_MMCP1.position, 1).SetEase(Ease.InExpo));
                                                sequence.Join(m_MMCEntity.transform.DOScale(m_MMCP1.localScale, 1).SetEase(Ease.InCubic));
                                                await sequence.AsyncWaitForCompletion();
                                                m_SceneEffect.gameObject.SetActive(false);
                                                m_LiHeModel.gameObject.SetActive(true);
                                                // 设置礼盒初始位置，从天空掉落
                                                Vector3 originalPosition = m_LiHeModel.transform.position;
                                                Vector3 startPosition = originalPosition + new Vector3(0, 8f, 0);
                                                Vector3 bouncePosition = originalPosition + new Vector3(0, 2.5f, 0);
                                                Vector3 floatPosition = originalPosition + new Vector3(0, 0.5f, 0);

                                                // 保存原始大小，用于弹跳效果
                                                Vector3 originalScale = m_LiHeModel.transform.localScale;
                                                Vector3 squashScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z * 1.2f);

                                                // 设置初始位置
                                                m_LiHeModel.transform.position = startPosition;

                                                // 创建掉落、弹跳和上升的动画序列
                                                DG.Tweening.Sequence giftBoxSequence = DOTween.Sequence();

                                                // 1. 掉落动画
                                                giftBoxSequence.Append(m_LiHeModel.transform.DOMove(bouncePosition, 0.6f).SetEase(Ease.InQuad));

                                                // 2. 挤压变形效果
                                                giftBoxSequence.Join(
                                                    DOTween.Sequence()
                                                        .AppendInterval(0.55f) // 接近地面时
                                                        .Append(m_LiHeModel.transform.DOScale(squashScale, 0.1f))
                                                );

                                                // 3. 回弹到地面
                                                // giftBoxSequence.Append(m_LiHeModel.transform.DOMove(originalPosition, 0.2f).SetEase(Ease.OutQuad));

                                                // 4. 恢复原始形状
                                                giftBoxSequence.Join(m_LiHeModel.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutElastic));

                                                // 5. 短暂停留
                                                giftBoxSequence.AppendInterval(0.5f);

                                                // 6. 缓缓上升并悬浮
                                                giftBoxSequence.Append(m_LiHeModel.transform.DOMove(floatPosition, 1.5f).SetEase(Ease.OutQuad));

                                                // 播放落地音效
                                                giftBoxSequence.InsertCallback(0.6f, () =>
                                                {
                                                    m_AudioManager.PlaySound("06-2-2", onComplete: () =>
                                                    {
                                                        this.DelaySeconds(0.5f, () =>
                                                        {
                                                            m_SuDiEntity.LookTarget = CameraPos;
                                                            m_SuNiEntity.LookTarget = CameraPos;
                                                            m_JBREntity.LookTarget = CameraPos;
                                                            m_AudioManager.PlaySound("06-4-2");
                                                                           m_LiHeModel.transform.DOMove(CameraPos.position - new Vector3(0, 1f, 0) + new Vector3(CameraPos.forward.x, 0, CameraPos.forward.z) * 3.0f, 2.0f)
                                                            .SetEase(Ease.OutQuad)
                                                            .OnComplete(() =>
                                                            {
                                                                giftBoxSequence.Kill(); // 停止动画
                                                                                        // 礼盒移动到玩家面前后，可以添加额外效果或逻辑
                                                                m_LiHeModel.transform
                                                                     .DOMove(m_LiHeModel.transform.position, 1.2f)
                                                                     .SetEase(Ease.InOutSine)
                                                                     .SetLoops(-1, LoopType.Yoyo); // 无限循环，来回运动

                                                                // 缓慢旋转效果
                                                                m_LiHeModel.transform
                                                                    .DORotate(new Vector3(0, 360, 0), 8f, RotateMode.LocalAxisAdd)
                                                                    .SetEase(Ease.Linear)
                                                                    .SetLoops(-1);
                                                            });
                                                        }, cancellationToken.Token).Forget();
                                                    });
                                                });
                                                // 7. 添加轻微的上下浮动效果
                                                // giftBoxSequence.OnComplete(() =>
                                                // {
                                                //     // 上下浮动循环
                                                //     m_LiHeModel.transform
                                                //         .DOMove(floatPosition + new Vector3(0, 0.3f, 0), 1.2f)
                                                //         .SetEase(Ease.InOutSine);
                                                //         // .SetLoops(-1, LoopType.Yoyo); // 无限循环，来回运动

                                                //     // 缓慢旋转效果
                                                //     m_LiHeModel.transform
                                                //         .DORotate(new Vector3(0, 360, 0), 8f, RotateMode.LocalAxisAdd)
                                                //         .SetEase(Ease.Linear);
                                                //         // .SetLoops(-1);
                                                // });

                                                // 执行动画序列
                                                giftBoxSequence.Play();



                                            }, cancellationToken.Token).Forget();
                                        });
                                    }, cancellationToken.Token).Forget();
                                });
                            }).Forget();
                        }).Forget();
                    }
                }).Forget();

            });

            // 两个实体都出场后，可以进行其他互动
            Debug.Log("姜饼人和苏迪已成功出场");
        }

        /// <summary>
        /// 处理手势事件
        /// </summary>
        /// <param name="palmEvent">手势事件数据</param>
        private void PalmEvent(PalmEvent palmEvent)
        {
            // 当手势状态有效、玩家在触发区域内并且射击功能已开启时尝试发射子弹
            if (palmEvent.status && playerInTriggerZone && canShoot)
            {
                // 使用射击系统发射子弹
                Vector3 spawnPosition = palmEvent.position;
                Quaternion spawnRotation = Quaternion.Euler(palmEvent.eulerAngles);

                shootingSystem.Shoot(spawnPosition, spawnRotation, m_MMCEntity?.AttackPoint.gameObject, "shoot");
            }
        }




        /// <summary>
        /// 销毁时取消事件订阅并清理资源
        /// </summary>
        void OnDestroy()
        {
            // 移除事件监听
            MessageDispatcher.RemoveListener<PalmEvent>(XY.UXR.API.OpenAPI.RKGesPalmEvent, PalmEvent);

            // 如果有触发器事件，也要移除相应的事件监听
            if (m_Enter != null && m_Enter.GetComponent<TriEvent>() != null)
            {
                m_Enter.GetComponent<TriEvent>().enterAction -= EnterEvent;
            }

            if (m_Exit != null && m_Exit.GetComponent<TriEvent>() != null)
            {
                m_Exit.GetComponent<TriEvent>().exitAction -= ExitEvent;
            }

            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
                cancellationToken = null;
            }

            Debug.Log("Node10007系统已清理");
        }

        /// <summary>
        /// 启用射击功能
        /// </summary>
        public void EnableShooting()
        {
            canShoot = true;
            Debug.Log("玩家射击功能已启用");

            // 可以添加音效或提示
            // if (m_AudioManager != null)
            // {
            //     m_AudioManager.PlaySound("enable_shooting");
            // }
        }

        /// <summary>
        /// 禁用射击功能
        /// </summary>
        public void DisableShooting()
        {
            canShoot = false;
            Debug.Log("玩家射击功能已禁用");
        }


#if UNITY_EDITOR
        /// <summary>
        /// OnGUI - 绘制测试按钮
        /// </summary>
        private void OnGUI()
        {
            if (!enableTestGUI) return;

            // 定义测试按钮的样式和位置
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20;
            buttonStyle.normal.textColor = Color.white;

            // 创建测试射击按钮
            if (GUI.Button(new Rect(testGUIOffsetX, testGUIOffsetY, 200, 60), "测试射击", buttonStyle))
            {
                if (shootingSystem != null && canShoot) // 检查是否已启用射击
                {
                    // 从相机位置发射子弹
                    Vector3 spawnPosition = mainCamera.transform.position;
                    Quaternion spawnRotation = mainCamera.transform.rotation;

                    // 调用射击系统发射子弹，目标为毛毛虫实体
                    shootingSystem.Shoot(spawnPosition, spawnRotation, m_MMCEntity?.AttackPoint.gameObject, "shoot");

                    Debug.Log("通过测试按钮触发射击");
                }
                else
                {
                    Debug.LogWarning(canShoot ? "射击系统未初始化，无法测试射击" : "射击功能尚未启用");
                }
            }

            // 创建切换射击状态的按钮
            string shootButtonText = canShoot ? "禁用射击功能" : "启用射击功能";
            if (GUI.Button(new Rect(testGUIOffsetX, testGUIOffsetY + 140, 200, 60), shootButtonText, buttonStyle))
            {
                if (canShoot)
                {
                    DisableShooting();
                }
                else
                {
                    EnableShooting();
                }
            }

            // 创建切换NPC射击状态的按钮
            if (enableNpcShooting && npcSDShootController != null && npcJBRShootController != null)
            {
                string npcButtonText = npcSDShootController.enableShooting ? "停止NPC射击" : "开始NPC射击";
                if (GUI.Button(new Rect(testGUIOffsetX, testGUIOffsetY + 70, 200, 60), npcButtonText, buttonStyle))
                {
                    npcSDShootController.enableShooting = !npcSDShootController.enableShooting;
                    npcJBRShootController.enableShooting = !npcJBRShootController.enableShooting;
                    Debug.Log($"NPC射击状态: {(npcSDShootController.enableShooting ? "已启用" : "已禁用")}");
                }
            }

            // 创建测试触发进入区域按钮
            if (GUI.Button(new Rect(testGUIOffsetX, testGUIOffsetY + 210, 200, 60), "触发进入区域", buttonStyle))
            {
                m_LiHeModel.gameObject.SetActive(true); 
                m_LiHeModel.transform.DOMove(CameraPos.position - new Vector3(0, 1f, 0) + new Vector3(CameraPos.forward.x, 0, CameraPos.forward.z) * 3.0f, 2.0f);
                if (!playerInTriggerZone)
                {
                    // EnterEvent();
                    Debug.Log("通过测试按钮触发进入区域事件");
                }
            }

            // 状态信息显示
            GUI.Label(new Rect(testGUIOffsetX, testGUIOffsetY + 280, 300, 30), $"玩家在区域内: {playerInTriggerZone}", buttonStyle);
            GUI.Label(new Rect(testGUIOffsetX, testGUIOffsetY + 310, 300, 30), $"射击功能状态: {(canShoot ? "已启用" : "已禁用")}", buttonStyle);
        }
#endif
    }
}