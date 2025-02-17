
using System.IO;
using UnityEditor;
using UnityEngine;

namespace D.Editor.Environment
{
    [InitializeOnLoad]
    public class DEnvFix : EditorWindow
    {
        private static DEnvFix window;
        private static FixBase[] fixItems;

        [MenuItem("D/Environment Fix", false)]
        public static void ShowWindow()
        {
            RegisterItems();
            window = GetWindow<DEnvFix>(true);
            window.minSize = new Vector2(320, 300);
            window.maxSize = new Vector2(320, 800);
            window.titleContent = new GUIContent("Environment Fix");
        }
        static DEnvFix()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            EditorApplication.delayCall -= OnDelayCall;
            EditorApplication.delayCall += OnDelayCall;
        }
        private static void OnUpdate()
        {
            Debug.Log("DEnvFix OnUpdate");
            EditorApplication.update -= OnUpdate;
        }
        private static void OnDelayCall()
        {
            if (fixItems == null) { RegisterItems(); }
            Debug.Log("DEnvFix OnDelayCall");
            // EditorApplication.delayCall -= OnDelayCall;
        }
        private static void RegisterItems()
        {
            fixItems = new FixBase[]
            {
                new FixEntity.CheckDefaultProjectData(MessageType.Warning),
                new FixEntity.CheckAPIConfiguration(MessageType.Warning),
                new FixEntity.CheckInputHandler(MessageType.Warning),
                new FixEntity.CheckBuildTarget(MessageType.Error),
                new FixEntity.CheckMiniumAPILevel(MessageType.Error)
            };
        }

        public void OnGUI()
        {
            GUILayout.Space(30);

            int validCount = 0;

            for (int i = 0; i < fixItems.Length; i++)
            {
                FixBase item = fixItems[i];

                bool valid = item.IsValid();

                if (!valid)
                {
                    validCount++;
                }
            }


            if (validCount > 0)
            {
                for (int i = 0; i < fixItems.Length; i++)
                {
                    FixBase item = fixItems[i];

                    if (!item.IsValid())
                    {
                        GUILayout.BeginVertical("box");
                        {
                            item.DrawGUI();

                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("Fix"))
                                    item.Fix();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
            }

            GUILayout.FlexibleSpace();

            if (validCount == 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("No issue found");

                        if (GUILayout.Button("Close Window"))
                            Close();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }
        }
    }
}