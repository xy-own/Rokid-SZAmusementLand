using Rokid.UXR.Interaction;

namespace XY.UXR.Gesture
{
    public class GesPinchClick : GestureBase
    {
        //int[] handStatus = new int[2];
        //private void Awake()
        //{
        //    BindEvent();
        //}
        //private void OnDestroy()
        //{
        //    UnBindEvent();
        //}
        //public override void OnTrackedSuccess(HandType handType)
        //{
        //    base.OnTrackedSuccess(handType);

        //    if (handType == HandType.LeftHand)
        //    {
        //        handStatus[0] = 1;
        //    }
        //    if (handType == HandType.RightHand)
        //    {
        //        handStatus[1] = 1;
        //    }
        //}
        //public override void OnTrackedFailed(HandType handType)
        //{
        //    base.OnTrackedFailed(handType);
        //    if (handType == HandType.None)
        //    {
        //        handStatus[0] = 0;
        //        handStatus[1] = 0;
        //    }
        //    if (handType == HandType.LeftHand)
        //    {
        //        handStatus[0] = 0;
        //    }
        //    if (handType == HandType.RightHand)
        //    {
        //        handStatus[1] = 0;
        //    }
        //}
        private void Update()
        {
            if(GesEventInput.Instance.GetHandDown(HandType.LeftHand)|| GesEventInput.Instance.GetHandDown(HandType.RightHand))
            {
                MessageDispatcher.SendMessageData(API.OpenAPI.RKGesPinchEvent);
            }
        }
    }
}