/*
* 文件名：ShortcutKeyWindow.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/15 15:25:01
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/
using UnityEditor;
using UnityEngine;

public class ShortcutKeyWindow : EditorWindow
{
    // 快捷键信息类
    public class ShortcutInfo
    {
        public string Name;
        public string Description;
        public string Shortcut;

        public ShortcutInfo(string name, string description, string shortcut)
        {
            Name = name;
            Description = description;
            Shortcut = shortcut;
        }
    }

    // 快捷键数据
    private static readonly ShortcutInfo[] shortcuts = new ShortcutInfo[]
    {
        new ShortcutInfo("播放/暂停", "开始或暂停游戏运行", "Ctrl + P (Windows) / Command + P (Mac)"),
        new ShortcutInfo("场景视图导航", "在场景视图中移动视角", "右键 + WASD"),
        new ShortcutInfo("最大化/最小化游戏视图", "将游戏视图最大化或最小化", "Shift + 空格"),
        new ShortcutInfo("保存场景", "保存当前场景", "Ctrl + S (Windows) / Command + S (Mac)"),
        new ShortcutInfo("撤销", "撤销上一步操作", "Ctrl + Z (Windows) / Command + Z (Mac)"),
        new ShortcutInfo("重做", "重做上一步撤销的操作", "Ctrl + Shift + Z (Windows) / Command + Shift + Z (Mac)"),
        new ShortcutInfo("切换2D/3D视图", "切换场景视图的2D或3D模式", "Ctrl + 2 (Windows) / Command + 2 (Mac)"),

        // 编辑物体相关快捷键
        new ShortcutInfo("移动物体", "在场景中移动选中的物体", "W 键"),
        new ShortcutInfo("旋转物体", "在场景中旋转选中的物体", "E 键"),
        new ShortcutInfo("缩放物体", "在场景中缩放选中的物体", "R 键"),
        new ShortcutInfo("选择物体", "选择当前物体或切换到物体选择模式", "Q 键"),
        new ShortcutInfo("复制物体", "复制选中的物体", "Ctrl + D (Windows) / Command + D (Mac)"),
        new ShortcutInfo("删除物体", "删除当前选中的物体", "Delete 键"),
        new ShortcutInfo("显示/隐藏 Gizmos", "显示或隐藏场景中的 Gizmos", "Ctrl + Shift + G (Windows) / Command + Shift + G (Mac)"),
        new ShortcutInfo("聚焦当前物体", "将场景视图聚焦到当前选中的物体", "F 键"),

        // 物体显示/隐藏相关快捷键
        new ShortcutInfo("显示/隐藏当前物体", "切换显示或隐藏当前选中的物体", "H 键"),

        // 场景操作相关
        new ShortcutInfo("切换场景视图和游戏视图", "在场景视图和游戏视图之间切换", "Ctrl + ` (Windows) / Command + ` (Mac)"),
        new ShortcutInfo("切换游戏视图", "在游戏视图和场景视图之间切换", "Shift + Space"),
        new ShortcutInfo("跳过一帧", "在运行模式下跳过一帧", "Ctrl + Alt + F (Windows) / Command + Option + F (Mac)"),

        // 其他快捷键
        new ShortcutInfo("打开控制台", "快速打开Unity控制台窗口", "Ctrl + Shift + C (Windows) / Command + Shift + C (Mac)"),
        new ShortcutInfo("打开偏好设置", "打开Unity的设置窗口", "Ctrl + , (Windows) / Command + , (Mac)")
    };

    // 滚动视图位置
    private Vector2 scrollPosition;

    // 通过菜单项来显示窗口
    [MenuItem("Window/快捷键指南")]
    public static void ShowWindow()
    {
        // 显示窗口
        ShortcutKeyWindow window = GetWindow<ShortcutKeyWindow>("快捷键指南");
        window.Show();
    }

    private void OnGUI()
    {
        // 设置标题样式
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        };

        GUILayout.Space(10);

        // 显示窗口的标题
        GUILayout.Label("常用快捷键", headerStyle);

        // 启用滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 40));

        foreach (var shortcut in shortcuts)
        {
            // 每个快捷键的显示格式
            GUILayout.BeginVertical("box");

            // 显示快捷键名称
            GUILayout.Label(shortcut.Name, EditorStyles.boldLabel);
            // 显示快捷键描述
            GUILayout.Label($"描述: {shortcut.Description}");
            // 显示快捷键组合
            GUILayout.Label($"快捷键: {shortcut.Shortcut}");

            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
    }

    // 检查快捷键并处理事件
    [InitializeOnLoadMethod]
    private static void CheckForShortcuts()
    {
        // 监听按键事件
        Event currentEvent = Event.current;

        if (currentEvent != null && currentEvent.type == EventType.KeyDown)
        {
            // 检查是否按下了 H 键
            if (currentEvent.keyCode == KeyCode.H)
            {
                ToggleObjectVisibility();
                currentEvent.Use();  // 确保不再向下传播
            }
        }
    }

    // 切换当前选中物体的显示/隐藏状态
    private static void ToggleObjectVisibility()
    {
        // 获取当前选中的物体
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            // 切换物体的激活状态（显示/隐藏）
            selectedObject.SetActive(!selectedObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("请先选择一个物体!");
        }
    }
}


