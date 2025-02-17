using UnityEditor;
using UnityEngine;

namespace D.Editor.ToolsEx
{

    [CustomEditor(typeof(CollisionViewer))]
    public class EnhancedCollisionViewerEditor : UnityEditor.Editor
    {
        EnhancedCollisionViewerEditor()
        {
            SceneView.duringSceneGui += OnViewGUI;
        }


        private void OnViewGUI(SceneView view)
        {
            Collider[] colliders = UnityEngine.Object.FindObjectsOfType<Collider>();

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out CollisionViewer viewer))
                {
                    DrawColliderBounds(collider, viewer);
                }
            }
        }

        private static void DrawColliderBounds(Collider collider, CollisionViewer viewer)
        {
            // 获取物体的 transform 和 bounds
            Transform transform = collider.transform;

            // 保存当前的坐标系矩阵
            Matrix4x4 originalMatrix = Handles.matrix;

            // 设置旋转的坐标系矩阵（仅影响旋转和缩放）
            Handles.matrix = transform.localToWorldMatrix;

            // 绘制备注文字
            if (viewer.showLabel)
            {
                GUIStyle labelStyle = new GUIStyle
                {
                    normal = new GUIStyleState { textColor = viewer.textColor },
                    fontSize = viewer.fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };

                // 计算物体头顶的局部位置（中心 + 上方偏移量）
                Vector3 labelOffset = Vector3.up * 0.5f; // 头顶偏移

                // 在物体头顶绘制文字
                Handles.Label(labelOffset, " " + viewer.note + " ", labelStyle);
            }

            // 恢复原本的矩阵
            Handles.matrix = originalMatrix;
        }
    }
}
