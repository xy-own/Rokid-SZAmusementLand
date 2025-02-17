using System;
using UnityEngine;

namespace D.Common
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Framework/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("是否走AB加载")]
        public bool abLoad = false;
        [Header("打印日志")]
        public bool logMode = false;
        [Header("显示帧频")]
        public bool showFps = false;
        [Header("运行帧频"), Range(30, 120)]
        public int gameFrameRate = 30;
    }
}