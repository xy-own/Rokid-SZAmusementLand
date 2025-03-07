using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace YZJ
{

    public class CollisionViewer : MonoBehaviour
    {

        [TextArea]
        [Header("备注信息")]
        public string note = "备注"; // 备注信息
        [Header("显示碰撞体")]
        public bool ColliderOpen = true;  // 
        public bool ColliderMeshOpen = true;  // 
        public bool ColliderLineOpen = true;  // 
        public Vector3 textPos = Vector3.zero;
        [Header("显示设置")]
        public Color meshColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);  // 文字颜色
        public Color textColor = Color.yellow;  // 文字颜色
        public Color backgroundColor = new Color(0, 0, 0, 0.5f);  // 文字背景颜色（半透明）
        [Range(10, 50)]
        public int fontSize = 20;               // 字体大小
        [Range(-10, 10)]
        public int fontHeight = 1;               // 字体高度

        [Header("可见性设置")]
        public bool showLabel = true;           // 是否显示备注
        public float minDisplaySize = 0.5f;     // 显示最小尺寸阈值
        public float maxDisplayDistance = 100f; // 最大显示距离

        [Header("性能优化")]
        public bool enableOptimization = true; // 是否启用优化
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Vector3 lastScale;

        private bool needsUpdate = true;

        private bool hasUpdated = false; // 标志，记录当前帧是否已更新

        private static GUIStyle sharedLabelStyle;

        private static Texture2D sharedBackgroundTexture;
#if UNITY_EDITOR
        /// <summary>
        /// 初始化共享的 GUIStyle 和 Texture2D
        /// </summary>
        private static void InitializeSharedResources()
        {
            if (sharedBackgroundTexture == null)
            {
                sharedBackgroundTexture = MakeTex(1, 1, new Color(0, 0, 0, 0.5f)); // 创建纹理
            }
            sharedLabelStyle = new GUIStyle
            {
                normal = new GUIStyleState { textColor = Color.white }, // 默认文字颜色
                fontSize = 12, // 默认字体大小
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            sharedLabelStyle.normal.background = sharedBackgroundTexture;
        }
        private void Awake()
        {
            InitializeSharedResources();
        }

        void Update()
        {
            if (enableOptimization)
            {
                if (!hasUpdated) // 仅在这一帧进行更新
                {
                    // 检查是否发生变换
                    if (transform.position != lastPosition ||
                        transform.rotation != lastRotation ||
                        transform.localScale != lastScale)
                    {
                        needsUpdate = true;
                        lastPosition = transform.position;
                        lastRotation = transform.rotation;
                        lastScale = transform.localScale;
                    }
                    else
                    {
                        needsUpdate = false;
                    }

                    hasUpdated = true; // 标记为已更新
                }
            }
            else
            {
                needsUpdate = true; // 如果未启用优化，始终更新
            }
        }
        void OnDrawGizmos()
        {
            if (!ColliderOpen || !needsUpdate) return;
            if (ColliderOpen)
            {
                InitializeSharedResources();
                // 获取附加到当前 GameObject 的所有碰撞体
                Collider[] colliders = GetComponents<Collider>();

                // 遍历每个碰撞体
                foreach (Collider col in colliders)
                {
                    if (col is BoxCollider)  // 如果是盒子碰撞体
                    {
                        BoxCollider box = (BoxCollider)col;
                        // 获取物体的缩放值
                        Vector3 objectScale = transform.localScale;

                        // 计算碰撞体的实际大小，考虑缩放
                        Vector3 actualSize = Vector3.Scale(box.size, objectScale);

                        // 获取物体的旋转值
                        Quaternion objectRotation = transform.localRotation;

                        if (ColliderLineOpen)
                        {
                            // 设置 Gizmos 颜色
                            Gizmos.color = textColor;
                            // 绘制包围盒线框
                            Gizmos.matrix = Matrix4x4.TRS(transform.position, objectRotation, Vector3.one);
                            Gizmos.DrawWireCube(box.center, actualSize);
                        }


                        if (ColliderMeshOpen)
                        {
                            // 绘制网格，基于计算的大小
                            MeshFilter meshFilter = GetComponent<MeshFilter>();
                            if (meshFilter != null && meshFilter.sharedMesh != null)
                            {
                                Gizmos.color = meshColor;
                                Gizmos.matrix = Matrix4x4.TRS(transform.position, objectRotation, actualSize);
                                Gizmos.DrawMesh(meshFilter.sharedMesh);
                            }
                        }

                    }
                    else if (col is SphereCollider)  // 如果是球形碰撞体
                    {
                        SphereCollider sphere = (SphereCollider)col;
                        // 获取物体的缩放值
                        Vector3 objectScale = transform.localScale;

                        // 计算碰撞体的实际大小，考虑缩放
                        float actualRadius = sphere.radius * Mathf.Max(objectScale.x, objectScale.y, objectScale.z);
                        // 获取物体的旋转值
                        Quaternion objectRotation = transform.localRotation;
                        if (ColliderLineOpen)
                        {
                            // 设置 Gizmos 颜色
                            Gizmos.color = textColor;
                            // 绘制包围球线框
                            Gizmos.DrawWireSphere(transform.position + sphere.center, actualRadius);
                        }


                        if (ColliderMeshOpen)
                        {
                            // 绘制网格，基于计算的半径
                            MeshFilter meshFilter = GetComponent<MeshFilter>();
                            if (meshFilter != null && meshFilter.sharedMesh != null)
                            {
                                Gizmos.color = meshColor;
                                Gizmos.matrix = Matrix4x4.TRS(transform.position, objectRotation, transform.localScale);
                                Gizmos.DrawMesh(meshFilter.sharedMesh);
                            }
                        }

                    }
                    else if (col is CapsuleCollider)  // 如果是胶囊体碰撞体
                    {
                        CapsuleCollider capsule = (CapsuleCollider)col;
                        // 获取物体的缩放值
                        Vector3 objectScale = transform.localScale;

                        // 计算实际的半径和高度，考虑物体的缩放
                        float actualRadius = capsule.radius * Mathf.Max(objectScale.x, objectScale.z); // X 和 Z 缩放影响半径
                        float actualHeight = capsule.height * objectScale.y; // Y 缩放影响高度

                        // 获取物体的旋转值
                        Quaternion objectRotation = transform.rotation;

                        // 设置 Gizmos 颜色
                        Gizmos.color = textColor;

                        // 计算胶囊体的基准中心点
                        Vector3 center = transform.position + capsule.center;

                        if (ColliderLineOpen)
                        {
                            Gizmos.color = meshColor;
                            // 计算胶囊体的上下端点
                            Vector3 top = center + objectRotation * Vector3.up * (actualHeight / 2);
                            Vector3 bottom = center - objectRotation * Vector3.up * (actualHeight / 2);
                            // 绘制胶囊体的上下圆（顶部和底部的半球）
                            Gizmos.DrawWireSphere(top, actualRadius);
                            Gizmos.DrawWireSphere(bottom, actualRadius);
                            // 绘制胶囊体的竖直线段（连接顶部和底部的主轴）
                            Gizmos.DrawLine(top, bottom);
                        }


                        if (ColliderMeshOpen)
                        {
                            // 绘制胶囊体网格
                            MeshFilter meshFilter = GetComponent<MeshFilter>();
                            if (meshFilter != null && meshFilter.sharedMesh != null)
                            {
                                Gizmos.color = meshColor;
                                Gizmos.matrix = Matrix4x4.TRS(transform.position, objectRotation, transform.localScale);
                                Gizmos.DrawMesh(meshFilter.sharedMesh);
                            }
                        }

                    }
                    else if (col is MeshCollider)  // 如果是 MeshCollider
                    {
                        MeshCollider meshCollider = (MeshCollider)col;

                        // 获取物体的缩放值
                        Vector3 objectScale = transform.localScale;

                        // 如果 MeshCollider 有网格，绘制它的包围盒
                        if (meshCollider.sharedMesh != null)
                        {
                            if (ColliderMeshOpen)
                            {
                                // 绘制 MeshCollider 网格
                                Gizmos.color = meshColor;
                                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
                                Gizmos.DrawMesh(meshCollider.sharedMesh);
                            }
                        }
                        else
                        {
                            if (ColliderLineOpen)
                            {
                                // 如果没有网格，显示一个警告（可选）
                                Gizmos.color = textColor;
                                Gizmos.DrawWireCube(transform.position, Vector3.one);
                                Debug.LogWarning("MeshCollider 没有附加有效的网格");
                            }

                        }
                    }
                    DrawColliderBounds(col, this);
                    // 可以根据需要添加更多碰撞体类型的处理
                }
            }
        }
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
                        background = sharedBackgroundTexture
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
            // 使用 HideFlags.DontSave 替代 HideAndDontSave
            Texture2D result = new Texture2D(width, height)
            {
                hideFlags = HideFlags.DontSave // 更安全的选项
            };
            // 手动填充颜色数组
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = col;
            }

            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
#endif
    }
}
