/*
* 文件名：AndroidBuildWindow.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/07/10 14:08:56
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YZJ
{
    // 用于存储应用信息的类
    public class AppInfo
    {
        public string appName;  // 应用名称
        public string packageName;  // 应用包名

        public AppInfo(string appName, string packageName)
        {
            this.appName = appName;
            this.packageName = packageName;
        }
    }

    /// <summary>
    /// 类：AndroidBuildWindow
    /// 描述：此类用于在Unity中切换应用名称和包名，并执行Android打包操作，同时动态选择场景进行打包。
    /// </summary>
    public class AndroidBuildWindow : EditorWindow
    {
        private static string versionNumber = "1.0.0";  // 版本号
        private static int versionCode = 1;  // 版本代码

        private List<AppInfo> appInfoList;  // 应用信息列表
        private int selectedAppIndex = 0;  // 当前选择的应用索引
        private List<EditorBuildSettingsScene> sceneList; // 当前打包的场景列表

        [MenuItem("工具/设置包名并构建Android")]
        public static void ShowWindow()
        {
            GetWindow<AndroidBuildWindow>("设置包名并构建Android");
        }

        private void OnEnable()
        {
            // 初始化应用信息列表
            appInfoList = new List<AppInfo>
            {
                new AppInfo("浙师大观摩空间", "com.xyani.zjnu.showroom")
            };

            // 加载当前版本信息
            LoadVersionInfo();

            // 加载当前打包场景
            sceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        }

        private void OnGUI()
        {
            GUILayout.Label("应用程序设置", EditorStyles.boldLabel);

            // 显示当前选择的应用项
            selectedAppIndex = EditorGUILayout.Popup("选择应用:", selectedAppIndex, GetAppNames());

            GUILayout.Space(10);

            // 显示当前应用信息
            GUILayout.Label("当前应用信息", EditorStyles.boldLabel);
            GUILayout.Label("应用名称: " + appInfoList[selectedAppIndex].appName);
            GUILayout.Label("包名: " + appInfoList[selectedAppIndex].packageName);
            versionNumber = EditorGUILayout.TextField("版本号:", versionNumber);
            GUILayout.Label("版本代码: " + versionCode);

            GUILayout.Space(10);

            // 场景选择列表
            GUILayout.Label("选择要打包的场景", EditorStyles.boldLabel);
            for (int i = 0; i < sceneList.Count; i++)
            {
                sceneList[i].enabled = EditorGUILayout.ToggleLeft(sceneList[i].path, sceneList[i].enabled);
            }

            GUILayout.Space(10);

            // 设置包名和应用名称按钮
            if (GUILayout.Button("设置应用名称和包名"))
            {
                SetPackageAndAppName(appInfoList[selectedAppIndex]);
            }

            GUILayout.Space(10);

            // 打包按钮
            if (GUILayout.Button("打包Android"))
            {
                // 在打包之前保存更新后的场景状态
                SaveSceneSettings();
                BuildAndroid(BuildOptions.None);
            }
            // 打包按钮
            if (GUILayout.Button("打包Android Build And Run"))
            {
                // 在打包之前保存更新后的场景状态
                SaveSceneSettings();
                BuildAndroid(BuildOptions.AutoRunPlayer);
            }
        }

        // 设置包名和应用名称
        private void SetPackageAndAppName(AppInfo appInfo)
        {
            // 设置应用名称
            PlayerSettings.productName = appInfo.appName;
            Debug.Log("应用名称设置为: " + appInfo.appName);

            // 设置包名
            PlayerSettings.applicationIdentifier = appInfo.packageName;
            Debug.Log("包名设置为: " + appInfo.packageName);

            // 保存修改
            AssetDatabase.SaveAssets();

            // 显示提示信息
            EditorUtility.DisplayDialog("设置成功", "应用名称和包名已成功设置！", "确定");
        }

        // 保存场景激活状态
        private void SaveSceneSettings()
        {
            EditorBuildSettings.scenes = sceneList.ToArray();
            Debug.Log("场景设置已更新");
        }

        // 获取所有应用名称的数组
        private string[] GetAppNames()
        {
            string[] appNames = new string[appInfoList.Count];
            for (int i = 0; i < appInfoList.Count; i++)
            {
                appNames[i] = appInfoList[i].appName;
            }
            return appNames;
        }

        // 执行打包操作
        private static void BuildAndroid(BuildOptions buildOptions)
        {
            // 加载版本信息
            LoadVersionInfo();

            // 获取用户选择的需要打包的场景
            string[] scenes = GetScenesToBuild();

            // 显示打包信息弹窗
            bool shouldBuild = ShowBuildInfo(scenes);

            if (shouldBuild)
            {
                // 执行打包操作
                PerformBuild(scenes, buildOptions);
            }
        }

        // 获取所有需要打包的场景
        private static string[] GetScenesToBuild()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }

        // 执行打包操作
        private static void PerformBuild(string[] scenes, BuildOptions buildOptions)
        {
            // 设置版本号和版本代码
            PlayerSettings.bundleVersion = versionNumber;
            PlayerSettings.Android.bundleVersionCode = versionCode;

            // 确保打包输出目录存在
            string buildPath = "Builds";
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            // 配置打包选项
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = Path.Combine(buildPath, $"浙师大观摩空间_v{versionNumber}_{versionCode}.apk"),
                target = BuildTarget.Android,
                options = buildOptions
            };

            // 执行打包
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            // 动态增加版本号
            IncrementVersionNumber();

            // 保存版本信息
            SaveVersionInfo();

            Debug.Log("打包成功！");
        }

        // 增加版本号
        private static void IncrementVersionNumber()
        {
            // 简单的版本号递增逻辑
            string[] parts = versionNumber.Split('.');
            int buildNumber;
            if (int.TryParse(parts[2], out buildNumber))
            {
                buildNumber++;
                // versionNumber = $"{parts[0]}.{parts[1]}.{buildNumber}";
            }

            // 增加版本代码
            versionCode++;
        }

        // 保存版本信息到文件
        private static void SaveVersionInfo()
        {
            System.IO.File.WriteAllText("Assets/version.txt", $"{versionNumber}\n{versionCode}");
        }

        // 从文件加载版本信息
        private static void LoadVersionInfo()
        {
            if (System.IO.File.Exists("Assets/version.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines("Assets/version.txt");
                if (lines.Length >= 2)
                {
                    // versionNumber = lines[0];
                    versionCode = int.Parse(lines[1]);
                }
            }
        }

        // 显示打包信息弹窗
        private static bool ShowBuildInfo(string[] scenes)
        {
            string sceneList = string.Join("\n", scenes);
            return EditorUtility.DisplayDialog("打包信息",
                $"以下场景将被打包:\n{sceneList}\n\n" +
                $"版本号: {versionNumber}\n" +
                $"版本代码: {versionCode}\n\n" +
                "是否继续进行打包？",
                "是", "否");
        }
    }
}
