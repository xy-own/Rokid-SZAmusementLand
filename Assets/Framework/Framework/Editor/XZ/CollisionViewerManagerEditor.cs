using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YZJ
{
    [InitializeOnLoad]
    public static class CollisionViewerManagerEditor
    {
        // 缓存所有带有 CollisionViewer 组件的 Viewer
        private static List<CollisionViewer> cachedViewers = new List<CollisionViewer>();

        // 共享的 GUIStyle 和 Texture2D，用于标签绘制
        private static GUIStyle sharedLabelStyle;
        private static Texture2D sharedBackgroundTexture;

        // 静态构造函数，在编辑器加载时初始化
        static CollisionViewerManagerEditor()
        {
            // 初始化共享资源
            // InitializeSharedResources();

            // // 初始化缓存
            // UpdateColliderCache();

            // // 订阅事件
            // SceneView.duringSceneGui += OnSceneGUI;
            // EditorApplication.hierarchyChanged += OnHierarchyChanged;
            // EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// 初始化共享的 GUIStyle 和 Texture2D
        /// </summary>
        private static void InitializeSharedResources()
        {
            // sharedLabelStyle = new GUIStyle
            // {
            //     normal = new GUIStyleState { textColor = Color.white }, // 默认文字颜色
            //     fontSize = 12, // 默认字体大小
            //     fontStyle = FontStyle.Bold,
            //     alignment = TextAnchor.MiddleCenter
            // };

            // sharedBackgroundTexture = MakeTex(1, 1, new Color(0, 0, 0, 0.5f)); // 半透明背景
            // sharedLabelStyle.normal.background = sharedBackgroundTexture;
        }

        /// <summary>
        /// 更新 Collider 缓存，确保所有缓存的 Viewer 都是有效的
        /// </summary>
        private static void UpdateColliderCache()
        {
            // 清空当前缓存
            // cachedViewers.Clear();

            // // 查找所有 CollisionViewer 组件
            // CollisionViewer[] allViewers = Object.FindObjectsOfType<CollisionViewer>();

            // foreach (CollisionViewer viewer in allViewers)
            // {
            //     if (viewer != null)
            //     {
            //         cachedViewers.Add(viewer);
            //     }
            // }
        }

        /// <summary>
        /// Hierarchy 变化回调，自动更新 Collider 缓存
        /// </summary>
        private static void OnHierarchyChanged()
        {
            // UpdateColliderCache();
            // SceneView.RepaintAll();
        }

        /// <summary>
        /// 处理 Play Mode 状态变化，确保在进入或退出播放模式时正确更新缓存
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
            // {
            //     UpdateColliderCache();
            //     SceneView.RepaintAll();
            // }
        }

        /// <summary>
        /// Scene GUI 绘制回调，用于绘制 Collider 边界和标签
        /// </summary>
        private static void OnSceneGUI(SceneView view)
        {
            // 创建一个临时列表来存储需要移除的 Viewer
            // List<CollisionViewer> viewersToRemove = new List<CollisionViewer>();

            // foreach (CollisionViewer viewer in cachedViewers)
            // {
            //     // 检查 Viewer 是否已被销毁
            //     if (viewer == null)
            //     {
            //         viewersToRemove.Add(viewer);
            //         continue;
            //     }

            //     // 获取 Viewer 关联的 Collider
            //     Collider collider = viewer.GetComponent<Collider>();
            //     if (collider == null)
            //         continue;

            //     // 判断是否需要显示
            //     if (ShouldDisplay(viewer, collider.bounds.size.magnitude, view))
            //     {
            //         DrawColliderBounds(collider, viewer);
            //     }
            // }

            // // 从缓存中移除已被销毁的 Viewer
            // if (viewersToRemove.Count > 0)
            // {
            //     foreach (var viewer in viewersToRemove)
            //     {
            //         cachedViewers.Remove(viewer);
            //     }
            // }
        }

        /// <summary>
        /// 判断是否需要显示 Collider 边界
        /// </summary>
        private static bool ShouldDisplay(CollisionViewer viewer, float colliderSize, SceneView view)
        {
            float distance = Vector3.Distance(view.camera.transform.position, viewer.transform.position);
            return (colliderSize >= viewer.minDisplaySize) && (distance <= viewer.maxDisplayDistance);
        }

        /// <summary>
        /// 绘制 Collider 边界和标签
        /// </summary>
        private static void DrawColliderBounds(Collider collider, CollisionViewer viewer)
        {
            // 获取物体的 transform 和 bounds
            Transform transform = collider.transform;
            Bounds bounds = collider.bounds;

            // 保存当前的坐标系矩阵
            Matrix4x4 originalMatrix = Handles.matrix;

            // 设置旋转的坐标系矩阵（仅影响旋转和缩放）
            Handles.matrix = transform.localToWorldMatrix;


            // 绘制备注文字
            if (viewer.showLabel)
            {
                // 创建基于 Viewer 设置的 GUIStyle
                GUIStyle labelStyle = new GUIStyle(sharedLabelStyle)
                {
                    normal = new GUIStyleState
                    {
                        textColor = viewer.textColor,
                        background = MakeTex(1, 1, viewer.backgroundColor)
                    },
                    fontSize = viewer.fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };

                // 计算物体头顶的偏移量
                Vector3 labelOffset = Vector3.up * 0.5f * viewer.fontHeight;

                // 在物体头顶绘制文字
                Handles.Label(labelOffset + viewer.textPos, $" {viewer.note} ", labelStyle);
            }

            // 恢复原本的矩阵
            Handles.matrix = originalMatrix;
        }

        /// <summary>
        /// 创建单色 Texture2D
        /// </summary>
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
