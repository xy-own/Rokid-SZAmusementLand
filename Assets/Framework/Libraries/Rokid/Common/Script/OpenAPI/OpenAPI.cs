namespace XY.UXR.API
{
    public class OpenAPI
    {
        /// <summary>
        /// 语音识别
        /// </summary>
        public const string RKVoice = "RKVoice";

        /// <summary>
        /// 投掷事件
        /// 对应的数据结构 XY.UXR.Gesture.ThrowInfo
        /// </summary>
        public const string RKGesThrow = "ThrowCtrl";

        /// <summary>
        /// 手掌事件
        /// 对应的数据结构 XY.UXR.Gesture.PalmEvent
        /// </summary>
        public const string RKGesPalmEvent = "RKGesPalmEvent";

        /// <summary>
        /// 捏合事件
        /// </summary>
        public const string RKGesPinchEvent = "RKGesPinchEvent";
    }
}