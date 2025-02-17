using System;
using UnityEngine;
namespace D.Utility
{
    public class KeyLongPress : MonoBehaviour
    {
        public Action action;

        [Tooltip("游戏按键锁")]
        [SerializeField]
        private float mGameLockTime = 5f;

        [Tooltip("键值")]
        [SerializeField]
        private KeyCode mKeyValue = KeyCode.None;
        private float mGameStart = 0f;


        public void Initialize(KeyCode keyCode, float clickTime = 5f)
        {
            mKeyValue = keyCode;
            mGameLockTime = clickTime;
        }
        void Update()
        {
            if (mKeyValue != KeyCode.None)
            {
                if (Input.GetKey(mKeyValue))
                {
                    mGameStart += Time.deltaTime;
                    if (mGameStart > mGameLockTime)
                    {
                        mGameStart = 0f;
                        action?.Invoke();
                    }
                }
                if (Input.GetKeyUp(mKeyValue)) // O
                {
                    mGameStart = 0f;
                }
            }
        }
    }
}