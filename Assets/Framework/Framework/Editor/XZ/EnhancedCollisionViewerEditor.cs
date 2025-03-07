using UnityEditor;
using UnityEngine;

namespace YZJ
{
    [CustomEditor(typeof(CollisionViewer))]
    public class EnhancedCollisionViewerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认 Inspector
            DrawDefaultInspector();

            EditorGUILayout.Space();

            // 提供信息提示
            EditorGUILayout.HelpBox("所有带有 CollisionViewer 组件的 Collider 会在 Scene 视图中显示其边界和标签。", MessageType.Info);
        }
    }
}
