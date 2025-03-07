using System;
using UnityEngine;
using UnityEngine.Events;

namespace SU10007
{
    /// <summary>
    /// 触发器事件类 - 用于检测物体进入和离开触发区域时执行相应动作
    /// </summary>
    public class TriEvent : MonoBehaviour
    {
        /// <summary>
        /// 当物体进入触发区域时执行的动作
        /// </summary>
        public Action enterAction = null;
        
        /// <summary>
        /// 当物体离开触发区域时执行的动作
        /// </summary>
        public Action exitAction = null;
        
        /// <summary>
        /// 当碰撞体进入触发区域时调用
        /// </summary>
        /// <param name="other">进入触发区域的碰撞体</param>
        private void OnTriggerEnter(Collider other)
        {
            // 检查进入的游戏对象是否带有"MainCamera"标签
            if (other.gameObject.tag.Equals("MainCamera"))
            {
                // 如果enterAction不为空，则调用该动作
                enterAction?.Invoke();
            }
        }
        
        /// <summary>
        /// 当碰撞体离开触发区域时调用
        /// </summary>
        /// <param name="other">离开触发区域的碰撞体</param>
        private void OnTriggerExit(Collider other)
        {
            // 检查离开的游戏对象是否带有"MainCamera"标签
            if (other.gameObject.tag.Equals("MainCamera"))
            {
                // 如果exitAction不为空，则调用该动作
                exitAction?.Invoke();
            }
        }
    }
}
