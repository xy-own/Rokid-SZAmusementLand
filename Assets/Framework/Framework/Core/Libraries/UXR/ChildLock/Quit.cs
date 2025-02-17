using UnityEngine;

namespace XY.UXR.ChildLock
{
    public class Quit : MonoBehaviour
    {
        [Header("搜索 DeviceEventHandler 脚本\n设置 ResponseToEscape 字段为false ")]
        [Space(10)]

        [Tooltip("childlock time")]
        [SerializeField]
        private float ChildLockTime = 3f;
        private float clickTime = 0f;

        [Tooltip("Station Pro Key")]
        [SerializeField]
        private KeyCode staionProKey0 = KeyCode.Escape;
        [Tooltip("Station Pro Key")]
        [SerializeField]
        private KeyCode staionProKey1 = KeyCode.JoystickButton2;

        [Tooltip("UXR RKKeyEvent")]
        [SerializeField]
        private Rokid.UXR.Module.RKKeyEvent uxrKey = Rokid.UXR.Module.RKKeyEvent.KEY_BACK;

        void Update()
        {
            if (Input.GetKey(staionProKey0) || Input.GetKey(staionProKey1) || Rokid.UXR.Module.RKNativeInput.Instance.GetKey(uxrKey))
            {
                clickTime += Time.deltaTime;
                if (clickTime > ChildLockTime)
                {
                    clickTime = 0f;
                    Application.Quit();
                    Rokid.UXR.Native.NativeInterface.NativeAPI.KillProcess();
                }
            }
            if (Input.GetKeyUp(staionProKey0) && Input.GetKeyUp(staionProKey1) && Rokid.UXR.Module.RKNativeInput.Instance.GetKeyUp(uxrKey))
            {
                clickTime = 0f;
            }
        }
    }
}