using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D.Editor.ToolsEx
{
    /// <summary>
    /// 一个用于创建装饰Unity内置编辑器类型的编辑器基类。
    /// </summary>
    public abstract class CustomCustomEditor : UnityEditor.Editor
    {
        // 定义一个空数组，用于方法调用时传递空参数。
        private static readonly object[] EMPTY_ARRAY = new object[0];

        // 缓存装饰的编辑器方法信息，减少反射调用的开销。
        private static readonly Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

        // 获取UnityEditor程序集，用于查找编辑器类型。
        private static readonly Assembly editorAssembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));

        // 构造函数，初始化装饰的编辑器类型并进行检查。
        protected CustomCustomEditor(string editorTypeName)
        {
            decoratedEditorType = editorAssembly.GetTypes().FirstOrDefault(t => t.Name == editorTypeName);
            Init();

            var originalEditedType = GetCustomEditorType(decoratedEditorType);
            if (originalEditedType != editedObjectType)
            {
                throw new ArgumentException(
                    string.Format("类型 {0} 与编辑器 {1} 类型 {2} 不匹配",
                                  editedObjectType, editorTypeName, originalEditedType));
            }
        }

        // 装饰的编辑器实例，懒加载模式创建实例。
        protected UnityEditor.Editor EditorInstance
        {
            get
            {
                if (editorInstance == null && targets != null && targets.Length > 0)
                {
                    editorInstance = CreateEditor(targets, decoratedEditorType);
                }

                if (editorInstance == null)
                {
                    Debug.LogError("无法创建编辑器实例！");
                }

                return editorInstance;
            }
        }

        // 获取装饰编辑器的自定义编辑类型。
        private static Type GetCustomEditorType(Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            CustomEditor[] attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
            if (attributes == null)
            {
                return null;
            }

            var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();
            return field.GetValue(attributes[0]) as Type;
        }

        // 初始化方法，用于设置被编辑的对象类型。
        private void Init()
        {
            editedObjectType = GetCustomEditorType(GetType());
        }

        // 在编辑器禁用时销毁实例，释放资源。
        private void OnDisable()
        {
            if (editorInstance != null)
            {
                DestroyImmediate(editorInstance);
            }
        }

        // 调用字段方法，通过反射调用目标字段的指定方法。
        protected void CallFieldMethod(string fieldName, string methodName, Type[] types, params object[] parameters)
        {
            Type type = EditorInstance.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            type = fieldInfo.FieldType;
            MethodInfo methodInfo = type.GetMethod(methodName, types);
            methodInfo.Invoke(fieldInfo.GetValue(EditorInstance), parameters);
        }

        // 调用装饰编辑器的方法，通过反射调用指定的方法。
        protected void CallInspectorMethod(string methodName, UnityEditor.Editor editor)
        {
            MethodInfo method = null;
            if (!decoratedMethods.ContainsKey(methodName))
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                method = decoratedEditorType.GetMethod(methodName, flags);

                if (method != null)
                {
                    decoratedMethods[methodName] = method;
                }
                else
                {
                    Debug.LogError(string.Format("无法找到方法 {0}", (MethodInfo)null));
                }
            }
            else
            {
                method = decoratedMethods[methodName];
            }

            if (method == null)
            {
                return;
            }

            method.Invoke(editor, EMPTY_ARRAY);
        }

        // 重载方法，简化调用。
        protected void CallInspectorMethod(string methodName)
        {
            CallInspectorMethod(methodName, editorInstance);
        }

        // 绘制场景GUI，调用装饰编辑器的OnSceneGUI方法。
        protected virtual void OnSceneGUI()
        {
            if (editorInstance)
            {
                CallInspectorMethod("OnSceneGUI");
            }
        }

        // 绘制头部GUI，调用装饰编辑器的OnHeaderGUI方法。
        protected override void OnHeaderGUI()
        {
            if (editorInstance)
            {
                CallInspectorMethod("OnHeaderGUI");
            }
        }

        // 绘制检查器GUI，调用装饰编辑器的OnInspectorGUI方法。
        public override void OnInspectorGUI()
        {
            EditorInstance.OnInspectorGUI();
        }

        // 绘制预览区域，调用装饰编辑器的DrawPreview方法。
        public override void DrawPreview(Rect previewArea)
        {
            EditorInstance.DrawPreview(previewArea);
        }

        // 获取信息字符串，调用装饰编辑器的GetInfoString方法。
        public override string GetInfoString()
        {
            return EditorInstance.GetInfoString();
        }

        // 获取预览标题，调用装饰编辑器的GetPreviewTitle方法。
        public override GUIContent GetPreviewTitle()
        {
            return EditorInstance.GetPreviewTitle();
        }

        // 检查是否有预览GUI，调用装饰编辑器的HasPreviewGUI方法。
        public override bool HasPreviewGUI()
        {
            return EditorInstance.HasPreviewGUI();
        }

        // 绘制交互式预览GUI，调用装饰编辑器的OnInteractivePreviewGUI方法。
        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            EditorInstance.OnInteractivePreviewGUI(r, background);
        }

        // 绘制预览GUI，调用装饰编辑器的OnPreviewGUI方法。
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            EditorInstance.OnPreviewGUI(r, background);
        }

        // 绘制预览设置，调用装饰编辑器的OnPreviewSettings方法。
        public override void OnPreviewSettings()
        {
            EditorInstance.OnPreviewSettings();
        }

        // 重新加载预览实例，调用装饰编辑器的ReloadPreviewInstances方法。
        public override void ReloadPreviewInstances()
        {
            EditorInstance.ReloadPreviewInstances();
        }

        // 渲染静态预览，调用装饰编辑器的RenderStaticPreview方法。
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        // 检查是否需要常量重绘，调用装饰编辑器的RequiresConstantRepaint方法。
        public override bool RequiresConstantRepaint()
        {
            return EditorInstance.RequiresConstantRepaint();
        }

        // 检查是否使用默认边距，调用装饰编辑器的UseDefaultMargins方法。
        public override bool UseDefaultMargins()
        {
            return EditorInstance.UseDefaultMargins();
        }

        #region Editor Fields
        private readonly Type decoratedEditorType; // 被装饰的编辑器类型
        private Type editedObjectType; // 被编辑的对象类型
        private UnityEditor.Editor editorInstance; // 装饰编辑器实例
        #endregion
    }
}
