using UnityEngine;
namespace D.Common
{
    public class ManagementCenter
    {
        /// <summary>
        /// 游戏管理器对象
        /// </summary>
        private static GameObject _mgrObj = null;
        public static GameObject mgrObj
        {
            get
            {
                if (_mgrObj == null)
                    _mgrObj = GameObject.FindWithTag("GameManager");
                return _mgrObj;
            }
        }
        private static Main _main = null;
        public static Main main
        {
            get
            {
                if (_main == null)
                {
                    _main = mgrObj.GetComponent<Main>();
                }
                return _main;
            }
        }
    }
}