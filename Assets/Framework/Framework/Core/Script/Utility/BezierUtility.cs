using System.Collections.Generic;
using UnityEngine;

namespace D.Utility
{
    public static class BezierUtility
    {
        internal static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            return u * u * p0 + 2 * t * u * p1 + tt * p2;

        }

        internal static Vector3 BezierIntepolate3(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            Vector3 p0p1 = Vector3.Lerp(p0, p1, t);
            Vector3 p1p2 = Vector3.Lerp(p1, p2, t);
            return Vector3.Lerp(p0p1, p1p2, t);

        }

        internal static Vector3 BeizerIntepolate4(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 p0p1 = Vector3.Lerp(p0, p1, t);
            Vector3 p1p2 = Vector3.Lerp(p1, p2, t);
            Vector3 p2p3 = Vector3.Lerp(p2, p3, t);

            Vector3 px = Vector3.Lerp(p0p1, p1p2, t);
            Vector3 py = Vector3.Lerp(p1p2, p2p3, t);

            return Vector3.Lerp(px, py, t);
        }

        internal static List<Vector3> BeizerIntepolate4List(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float pointCount)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (int i = 0; i < pointCount; i++)
            {
                pointList.Add(BeizerIntepolate4(p0, p1, p2, p3, i / pointCount));
            }
            return pointList;
        }

        internal static Vector3 GetP2AB(Vector3 a, Vector3 b, Vector3 p)
        {
            Vector3 V1 = p - a;
            Vector3 V2 = b - a;
            Vector3 ty = Vector3.Project(V1, V2);
            Vector3 normal = ty - V1;
            return normal + p;
        }

        internal static bool IsPointOnLine(Vector3 a, Vector3 b, Vector3 p)
        {
            float distanceAP = Vector3.Distance(a, p);
            float distanceBP = Vector3.Distance(b, p);
            float distanceAB = Vector3.Distance(a, b);

            if (Mathf.Approximately(distanceAP + distanceBP, distanceAB))
                return true;
            return false;
        }
    }
}