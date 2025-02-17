using UnityEngine;
using XY.UXR.Gesture.Button;
namespace XY.UXR.Gesture.Trigger
{
    public class TriggerItem : MonoBehaviour
    {
        private DownwardEvent downwardEvent = new DownwardEvent();
        private Vector3 start = Vector3.zero;
        private bool isTrigger = false;
        private int handCount = 0;

        public void Enter(Vector3 point, int count)
        {
            start = point;
            isTrigger = true;
            handCount = count;
        }
        public void Exit(Vector3 point, int count)
        {
            if (isTrigger)
            {
                isTrigger = false;
                bool bl = handCount == count && handCount == 2;

                downwardEvent.handCount = bl ? 2 : 1;
                downwardEvent.startPoint = start;
                downwardEvent.endPoint = point;
                MessageDispatcher.SendMessageData(XY.UXR.API.OpenAPI.GesDownwardEvent, downwardEvent);
            }
        }
    }
}