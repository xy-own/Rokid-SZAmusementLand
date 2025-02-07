using UnityEngine;

namespace XY.UXR
{
    public class PositionUtils
    {
        /// <summary>
        /// 通过三个点确定一个平面
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Plane HandPanel(Vector3 vector1, Vector3 vector2, Vector3 vector3)
        {
            Vector3 AB = vector2 - vector1;
            Vector3 AC = vector3 - vector1;
            Vector3 normal = Vector3.Cross(AB, AC).normalized;
            return new Plane(normal, vector1);
        }

        /// <summary>
        /// 在相机坐标系下
        /// 手腕（wrist）、拇指基部（base of thumb）、小指基部（base of pinky）的关键点。
        /// </summary>
        /// <param name="thumbBase"></param>
        /// <param name="palmCenter"></param>
        /// <param name="wrist"></param>
        /// <returns></returns>
        public static (Vector3, Quaternion) HanQuaternion(Vector3 thumbBase, Vector3 pinkBase, Vector3 wrist,
            int handType)
        {
            // 面向用户的修正
            //thumbBase = Player.Instance.transform.InverseTransformPoint(thumbBase);
            //pinkBase = Player.Instance.transform.InverseTransformPoint(pinkBase);
            //wrist = Player.Instance.transform.InverseTransformPoint(wrist);
            Vector3 wristToThumb = thumbBase - wrist;
            Vector3 wristToPink = pinkBase - wrist;
            Vector3 palm_normal = Vector3.Cross(wristToPink, wristToThumb).normalized;
            if (handType == 1)
            {
                palm_normal = Vector3.Cross( wristToPink,wristToThumb).normalized;
            }

            Quaternion rotation = Quaternion.FromToRotation(palm_normal, Vector3.up);
            return (palm_normal, rotation);
        }

        public static float ConvertTo180(float angle)
        {
            if (angle > 180f)
                return angle - 360f;
            return angle;
        }
    }
}