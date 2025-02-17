using UnityEngine;
using D.Base;
using D.Define;
using D.Utility;

namespace D
{
    public class Main : MonoBehaviour
    {
        private void Awake()
        {
            InitSettings();
            InitDebug();
            DG.Tweening.DOTween.Init(true, true, DG.Tweening.LogBehaviour.Default);
            InitModule();
        }
        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitSettings()
        {
            Common.GameSettings settings = Util.LoadGameSettings();
            if (settings != null)
            {
                AppConst.LogMode = settings.logMode;
                AppConst.ShowFps = settings.showFps;
                AppConst.GameFrameRate = settings.gameFrameRate;
                AppConst.ABLoad = settings.abLoad;
            }
        }
        /// <summary>
        /// 添加调试
        /// </summary>
        private void InitDebug()
        {
            if (AppConst.ShowFps)
            {
                gameObject.AddComponent<AppFPS>();
            }
            if (AppConst.LogMode)
            {
                gameObject.AddComponent<GameCMD>();
            }
            Application.targetFrameRate = AppConst.GameFrameRate;

            // 设置应用不休眠
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        /// <summary>
        /// 启动
        /// </summary>
        private void InitModule()
        {
            InitMono();
        }



        [SerializeField]
        private MonoBase mMonoBase = null;
        private void InitMono()
        {
            mMonoBase?.OnEnter(string.Empty);
        }
    }
}