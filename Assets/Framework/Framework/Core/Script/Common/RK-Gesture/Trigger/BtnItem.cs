using System;
using UnityEngine;
namespace D
{
    public class BtnItem : MonoBehaviour
    {
        public Action<GameObject> enterAction;
        public Action<GameObject> exitAction;

        public void Enter()
        {
            enterAction?.Invoke(gameObject);
        }
        public void Exit()
        {
            exitAction?.Invoke(gameObject);
        }
    }
}