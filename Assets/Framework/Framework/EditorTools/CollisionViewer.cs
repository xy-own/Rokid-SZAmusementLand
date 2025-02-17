using UnityEngine;

namespace D.Editor.ToolsEx
{
#if UNITY_EDITOR
    public class CollisionViewer : MonoBehaviour
    {
        [TextArea]
        [Header("显示文本")]
        public string note = "Title"; // 备注信息

        [Header("字体颜色")]
        public Color textColor = Color.yellow;  // 文字颜色

        [Header("字体大小")]
        [Range(10, 50)]
        public int fontSize = 20;               // 字体大小

        [Header("是否显示")]
        public bool showLabel = true;           // 是否显示备注
    }
#endif
}