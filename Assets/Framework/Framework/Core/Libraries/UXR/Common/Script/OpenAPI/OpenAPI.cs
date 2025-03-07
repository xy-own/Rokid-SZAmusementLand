namespace XY.UXR.API
{
    public class OpenAPI
    {
        /// <summary>
        /// 语音事件
        /// 对应的数据结构 string
        /// </summary>
        public const string RKVoice = "RKVoice";

        /// <summary>
        /// 手掌事件
        /// 对应的数据结构 XY.UXR.Gesture.PalmEvent
        /// </summary>
        public const string RKGesPalmEvent = "RKGesPalmEvent";

        /// <summary>
        /// 手掌事件
        /// 对应的数据结构 XY.UXR.Gesture.PalmEvent
        /// </summary>
        public const string RKGesPalmEvent1 = "RKGesPalmEvent1";

        /// <summary>
        /// 手掌往下挥动事件
        /// 对应的数据结构 XY.UXR.Gesture.Trigger.DownwardEvent
        /// </summary>
        public const string GesDownwardEvent = "GesDownwardEvent";

        /// <summary>
        /// 握拳事件
        /// 对应的数据结构 XY.UXR.Gesture.HandBackEvent
        /// </summary>
        public const string RKHandGripEvent = "RKHandGripEvent";

        /// <summary>
        /// 手背事件
        /// 对应的数据结构 XY.UXR.Gesture.HandBackEvent
        /// </summary>
        public const string RKHandBackEvent = "RKHandBackEvent";

        /// <summary>
        /// 捏合事件
        /// 对应的数据结构 XY.UXR.Gesture.RKGesPinchEvent
        /// </summary>
        public const string RKGesPinchEvent = "RKGesPinchEvent";
        /// <summary>
        /// 打开捏合事件
        /// 对应的数据结构 XY.UXR.Gesture.RKGesPinchEvent
        /// </summary>
        public const string RKGesOpenPinchEvent = "RKGesOpenPinchEvent";
        /// <summary>
        /// 投掷事件
        /// 对应的数据结构 XY.UXR.Gesture.ThrowInfo
        /// </summary>
        public const string RKGesThrow = "ThrowCtrl";
        /// <summary>
        /// 事件
        /// 对应的数据结构 XY.UXR.Gesture.PalmEvent
        /// </summary>
        public const string ScissorsEvent = "RKScissorsEvent";
    }
}