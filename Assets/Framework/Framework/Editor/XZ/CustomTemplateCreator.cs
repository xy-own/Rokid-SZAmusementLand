using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CustomTemplateCreator : EditorWindow
{
    private static TemplateConfig templateConfig;
    private static List<string> templateNames;
    private int selectedTemplateIndex = 0;
    private string newScriptName = "NewScript";
    private string newScriptDescription = "描述";

    [MenuItem("Assets/Create/Framework/创建模板", false, 80)]
    public static void ShowWindow()
    {
        LoadTemplateConfig();
        if (templateConfig == null || templateConfig.templates == null || templateConfig.templates.Count == 0)
        {
            Debug.LogError("TemplateConfig中没有找到任何模板。");
            return;
        }

        templateNames = new List<string>();
        foreach (var template in templateConfig.templates)
        {
            templateNames.Add(template.name);
        }

        CustomTemplateCreator window = GetWindow<CustomTemplateCreator>("创建模板");
        window.Show();
    }

    private static void LoadTemplateConfig()
    {
        if (templateConfig != null) return; // 配置已加载

        string[] guids = AssetDatabase.FindAssets("t:TemplateConfig");
        if (guids.Length == 0)
        {
            Debug.LogError("未找到TemplateConfig资产。");
            return;
        }

        string configPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        templateConfig = AssetDatabase.LoadAssetAtPath<TemplateConfig>(configPath);
        if (templateConfig == null)
        {
            Debug.LogError("无法从路径加载TemplateConfig: " + configPath);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("选择模板", EditorStyles.boldLabel);
        selectedTemplateIndex = EditorGUILayout.Popup(selectedTemplateIndex, templateNames.ToArray());

        GUILayout.Label("输入脚本名称", EditorStyles.boldLabel);
        newScriptName = EditorGUILayout.TextField(newScriptName);

        GUILayout.Label("输入脚本描述", EditorStyles.boldLabel);
        newScriptDescription = EditorGUILayout.TextField(newScriptDescription);

        if (GUILayout.Button("创建"))
        {
            CreateTemplate(templateNames[selectedTemplateIndex], newScriptName, newScriptDescription);
            this.Close();
        }
    }

    private static void CreateTemplate(string templateName, string scriptName, string scriptDescription)
    {
        TemplateConfig.TemplateInfo templateInfo = templateConfig.templates.Find(t => t.name == templateName);
        if (templateInfo == null)
        {
            Debug.LogError("未找到模板: " + templateName);
            return;
        }

        TextAsset templateFile = templateInfo.templateFile;
        Texture2D icon = templateInfo.icon;
        string templateContent = templateFile.text;
        string defaultFileName = scriptName +templateInfo.name + ".cs";

        if (icon == null)
        {
            Debug.LogError("未找到模板的图标: " + templateName);
        }

        var createAction = ScriptableObject.CreateInstance<DoCreateTemplate>();
        createAction.templateContent = templateContent;
        createAction.templateName = templateName;
        createAction.scriptDescription = scriptDescription;

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            createAction,
            defaultFileName,
            icon,
            AssetDatabase.GetAssetPath(templateFile)
        );
    }

    private class DoCreateTemplate : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        public string templateContent;
        public string templateName;
        public string scriptDescription;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //添加模板名称
            // pathName = Path.GetFileNameWithoutExtension(pathName);

            // 获取脚本文件名（不带扩展名）
            string fileName = Path.GetFileNameWithoutExtension(pathName);

            // 替换脚本名称标记
            templateContent = templateContent.Replace("#SCRIPTNAME#", fileName);
            templateContent = templateContent.Replace("#TEMPLATENAME#", templateName);
            templateContent = templateContent.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            templateContent = templateContent.Replace("#DESCRIPTION#", scriptDescription);

            // 创建并写入新脚本文件
            File.WriteAllText(pathName, templateContent);

            // 刷新AssetDatabase
            AssetDatabase.Refresh();

            // 选中新创建的脚本文件
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            Selection.activeObject = obj;

            // 设置图标
            TemplateConfig.TemplateInfo templateInfo = templateConfig.templates.Find(t => t.templateFile.name == Path.GetFileNameWithoutExtension(resourceFile));
            if (templateInfo != null && templateInfo.icon != null)
            {
                EditorGUIUtility.SetIconForObject(obj, templateInfo.icon);
            }
        }
    }
}
