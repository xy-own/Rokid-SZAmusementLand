using Rokid.UXR.Interaction;

namespace XY.UXR.Gesture
{
    public class ClickEvent
    {
        /// <summary>
        /// 当前手势状态
        /// </summary>
        public GestureType status;
    }
    public class GesClick : GestureBase
    {
        private void Awake()
        {
            BindEvent();
        }
        private void OnDestroy()
        {
            UnBindEvent();
        }
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);
            if(handType!= HandType.None)
            {
                GestureType type = (GestureType)gestureBean.gesture_type;
                MessageDispatcher.SendMessageData(API.OpenAPI.RKGesPinchEvent, type);
            }
        }
    }
}