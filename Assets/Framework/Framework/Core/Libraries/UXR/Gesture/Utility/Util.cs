using System;
using UnityEngine;

namespace XY.UXR.Gesture
{
    public static class Util
    {
        public static Vector3 GetHandOffsetPosition(Vector3 pos, Vector3 rot, float offset)
        {
            Vector3 forward = Quaternion.Euler(rot) * Vector3.forward;
            Vector3 newPosition = pos + forward * offset;
            return newPosition;
        }
        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }
    }
}