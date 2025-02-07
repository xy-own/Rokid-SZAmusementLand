using Rokid.UXR.Interaction;
using UnityEngine;
using System.Collections.Generic;

namespace XY.UXR
{
    public class GestureManager : MonoBehaviour
    {
        private static readonly object npcLock = new object();
        static List<GestureBase> Managers = new List<GestureBase>();
        private void Awake()
        {
            GesEventInput.OnTrackedSuccess += OnTrackedSuccess;
            GesEventInput.OnTrackedFailed += OnTrackedFailed;
            GesEventInput.OnRenderHand += OnRenderHand;
        }
        private void OnDestroy()
        {
            GesEventInput.OnTrackedSuccess -= OnTrackedSuccess;
            GesEventInput.OnTrackedFailed -= OnTrackedFailed;
            GesEventInput.OnRenderHand -= OnRenderHand;
        }
        void OnTrackedSuccess(HandType handType)
        {
            if (Managers.Count < 1)
                return;
            foreach (GestureBase item in Managers)
            {
                item.OnTrackedSuccess(handType);
            }
        }
        void OnTrackedFailed(HandType handType)
        {
            if (Managers.Count < 1)
                return;
            foreach (GestureBase item in Managers)
            {
                item.OnTrackedFailed(handType);
            }
        }
        void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            if (Managers.Count < 1)
                return;
            if (gestureBean == null)
            {
                return;
            }
            foreach (GestureBase item in Managers)
            {
                item.OnRenderHand(handType, gestureBean);
            }
        }
        public static GestureBase AddManager(GestureBase ges)
        {
            lock (npcLock)
            {
                if (Managers.Contains(ges))
                    return null;
                Managers.Add(ges);
            }
            return ges;
        }
        public static void RemoveManager(GestureBase ges)
        {
            lock (npcLock)
            {
                if (!Managers.Contains(ges))
                {
                    return;
                }
                Managers.Remove(ges);
            }
        }
    }
}