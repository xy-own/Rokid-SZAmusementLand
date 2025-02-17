using UnityEditor;

namespace D.Editor.Environment
{
    public class FixEntity
    {
        #region Default Project Data
        public class CheckDefaultProjectData : FixBase
        {
            public CheckDefaultProjectData(MessageType level) : base(level)
            {
                IsValid();
            }

            public override void DrawGUI()
            {
                string message = $"Init Default Project Data";
                DrawContent("Default Project Data", message);
            }

            public override void Fix()
            {
                PlayerSettings.companyName = Const.companyName;
                PlayerSettings.productName = Const.productName;
                PlayerSettings.bundleVersion = Const.version;
                PlayerSettings.Android.bundleVersionCode = Const.bundleVersionCode;

                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, Const.packageName);
            }

            public bool isFix = false;
            public override bool IsValid()
            {
                if (isFix)
                    return isFix;

                bool name = PlayerSettings.companyName.Equals(Const.companyName);

                bool productName = PlayerSettings.productName.Equals(Const.productName);

                bool version = false;
                string str = PlayerSettings.bundleVersion.Replace(".", "");
                if (!string.IsNullOrEmpty(str) && int.TryParse(str, out int number) && number >= Const.bundleVersionCode)
                {
                    version = true;
                }

                bool versionCode = PlayerSettings.Android.bundleVersionCode >= Const.bundleVersionCode;

                bool packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) == Const.packageName;

                isFix = name && productName && version && versionCode && packageName;
                return isFix;
            }
        }
        #endregion

        #region BuildTarget
        public class CheckBuildTarget : FixBase
        {
            public CheckBuildTarget(MessageType level) : base(level)
            {
                IsValid();
            }

            public override void DrawGUI()
            {
                string message = $"The Build Target should be Android";
                DrawContent("Build Target", message);
            }

            public override void Fix()
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            public bool isFix = false;
            public override bool IsValid()
            {
                if (isFix)
                    return isFix;
                isFix = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                return isFix;
            }
        }
        #endregion

        #region Android MinSdkVersion
        public class CheckMiniumAPILevel : FixBase
        {
            public CheckMiniumAPILevel(MessageType level) : base(level)
            {
                IsValid();
            }

            public override void DrawGUI()
            {
                string message = $"The minSdkVersion needs to be {Const.minSdkVersion}";
                DrawContent("MinSDKVersion", message);
            }

            public override void Fix()
            {
                PlayerSettings.Android.minSdkVersion = Const.minSdkVersion;
            }

            public bool isFix = false;
            public override bool IsValid()
            {
                if (isFix)
                    return isFix;
                isFix = PlayerSettings.Android.minSdkVersion >= Const.minSdkVersion;
                return isFix;
            }
        }
        #endregion


        #region CheckAPIConfiguration
        public class CheckAPIConfiguration : FixBase
        {
            public CheckAPIConfiguration(MessageType level) : base(level)
            {
                IsValid();
            }

            public override void DrawGUI()
            {
                string message = $"The ApiCompatibilityLevel should be {Const.apiCompatibilityLevel}\n\nThe ScriptingBackend should be {Const.scriptingImplementation}";
                DrawContent("APIConfiguration", message);
            }

            public override void Fix()
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, Const.apiCompatibilityLevel);
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, Const.scriptingImplementation);
            }

            public bool isFix = false;
            public override bool IsValid()
            {
                if (isFix)
                    return isFix;
                isFix = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) == Const.apiCompatibilityLevel && PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == Const.scriptingImplementation;
                return isFix;
            }
        }
        #endregion


        #region Check InputHandler
        public class CheckInputHandler : FixBase
        {
            public CheckInputHandler(MessageType level) : base(level)
            {
                IsValid();
            }

            public override void DrawGUI()
            {
                string message = $"The InputHandler should be Both";
                DrawContent("InputHandler", message);
            }

            public override void Fix()
            {
            }

            public bool isFix = false;
            public override bool IsValid()
            {
                if (isFix)
                    return isFix;
                isFix = !isFix;
                return isFix;
            }
        }
        #endregion
    }
}