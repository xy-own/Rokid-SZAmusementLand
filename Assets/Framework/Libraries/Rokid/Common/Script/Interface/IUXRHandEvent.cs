using Rokid.UXR.Interaction;

namespace XY.UXR
{
    public interface IUXRHandEvent
    {
        public void OnTrackedSuccess(HandType handType);
        public void OnTrackedFailed(HandType handType);
        public void OnRenderHand(HandType handType, GestureBean gestureBean);
    }
}