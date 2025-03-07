/*
* 文件名：MonoBehaviourExtensions.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/03 00:22:25
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace SU10007
{
    /// <summary>
    /// 类：MonoBehaviourExtensions
    /// 描述：此类的功能和用途...
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// 延迟指定的时间，单位为秒。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        /// <param name="seconds">延迟时间，单位为秒。</param>
        public static UniTask DelaySeconds(this MonoBehaviour monoBehaviour, float seconds, CancellationToken token = default)
        {
            try
            {
                // 检查任务是否已经取消
                if (token.IsCancellationRequested)
                {
                    Debug.Log("异步任务已被取消，不执行回调函数");
                    return UniTask.CompletedTask; // 如果已取消，直接返回
                }

                // 返回带有 CancellationToken 的延迟任务
                return UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // 捕获任务取消异常
                Debug.Log("异步任务已被取消，未执行延迟操作");
                return UniTask.CompletedTask; // 返回已完成的任务
            }
            catch (Exception ex)
            {
                // 捕获其他未预期的异常
                Debug.LogError($"执行延迟任务时发生异常: {ex.Message}");
                return UniTask.CompletedTask; // 确保即使发生异常，也返回已完成的任务
            }
        }

        public static async UniTask DelaySeconds(this MonoBehaviour monoBehaviour, float seconds, Action action, CancellationToken token = default)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    Debug.Log("异步已被取消，不执行回调函数");
                    return;
                }
                // 延迟指定的时间，并支持取消
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);

                // 在延迟完成后执行回调
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // 任务被取消时的处理
                Debug.Log("异步任务已被取消，未执行回调函数");
            }
            catch (Exception ex)
            {
                // 捕获其他未预期的异常
                Debug.LogError($"执行延迟任务时发生异常: {ex.Message}");
            }
        }
        /// <summary>
        /// 启动一个协程，并返回该协程。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        /// <param name="routine">协程方法。</param>
        /// <returns>返回启动的协程。</returns>
        public static Coroutine StartCoroutine(this MonoBehaviour monoBehaviour, IEnumerator routine)
        {
            return monoBehaviour.StartCoroutine(routine);
        }

        /// <summary>
        /// 停止一个协程。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        /// <param name="routine">要停止的协程。</param>
        public static void StopCoroutine(this MonoBehaviour monoBehaviour, Coroutine routine)
        {
            monoBehaviour.StopCoroutine(routine);
        }

        /// <summary>
        /// 启用一个 GameObject。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        public static void Enable(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour.gameObject.SetActive(true);
        }

        /// <summary>
        /// 禁用一个 GameObject。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        public static void Disable(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour.gameObject.SetActive(false);
        }

        /// <summary>
        /// 延迟指定的时间后执行一个操作，单位为秒。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        /// <param name="seconds">延迟时间，单位为秒。</param>
        /// <param name="action">延迟后要执行的操作。</param>
        public static async void InvokeAfterDelay(this MonoBehaviour monoBehaviour, float seconds, Action action, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("异步任务已被取消，不执行回调函数");
                return;
            }
            await monoBehaviour.DelaySeconds(seconds, token);
            action?.Invoke();
        }

        /// <summary>
        /// 重复调用一个方法，每隔指定时间，单位为秒。
        /// </summary>
        /// <param name="monoBehaviour">当前 MonoBehaviour 实例。</param>
        /// <param name="seconds">间隔时间，单位为秒。</param>
        /// <param name="action">要重复调用的方法。</param>
        /// <returns>返回启动的协程。</returns>
        public static Coroutine InvokeRepeating(this MonoBehaviour monoBehaviour, float seconds, Action action)
        {
            return monoBehaviour.StartCoroutine(InvokeRepeatingCoroutine(seconds, action));
        }

        private static IEnumerator InvokeRepeatingCoroutine(float seconds, Action action)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                action?.Invoke();
            }
        }
    }
}

