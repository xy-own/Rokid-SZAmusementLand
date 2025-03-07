/*
* 文件名：AudioManager.cs
* 作者：优化版本
* Unity版本：2022.3 LTS
* 创建日期：2023-10-01
* 版权：© 2023 Rokid
* All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

namespace SU10007
{
    /// <summary>
    /// 类：AudioManager
    /// 描述：音频管理器，负责处理游戏中的音频播放、停止和管理
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private AudioSource mAudioSource = null;            // 音频播放组件
        private Dictionary<string, AudioClip> audioCache;   // 音频缓存，避免重复加载
        private bool isInitialized = false;                 // 初始化标志

        private Coroutine checkCompletionCoroutine;        // 检测音频播放完成的协程
        private Action onAudioCompleted;                   // 音频播放完成回调

        /// <summary>
        /// 构造函数 - 使用已有的AudioSource初始化音频管理器
        /// </summary>
        /// <param name="source">要使用的音频源组件</param>
        public void Awake()
        {
            mAudioSource = this.AddComponent<AudioSource>();
            audioCache = new Dictionary<string, AudioClip>();
            isInitialized = true;
            Debug.Log("音频管理器初始化成功");
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="name">音频资源名称</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="onComplete">播放完成后的回调</param>
        /// <returns>是否成功播放</returns>
        public bool PlaySound(string name, bool loop = false, Action onComplete = null)
        {
            if (!CheckInitialization()) return false;

            // 停止之前的检测协程
            StopCheckCompletionCoroutine();

            AudioClip clip = GetAudioClip(name);
            if (clip == null) return false;

            mAudioSource.loop = loop;
            mAudioSource.clip = clip;
            mAudioSource.Play();
            Debug.Log($"正在播放音频: {name}, 循环: {loop}");

            // 设置回调并启动检测协程
            onAudioCompleted = onComplete;
            if (!loop && onComplete != null) // 循环播放不需要回调
            {
                checkCompletionCoroutine = StartCoroutine(CheckAudioCompletion());
            }

            return true;
        }

        /// <summary>
        /// 播放一次性音频（不会中断当前正在播放的音频）
        /// </summary>
        /// <param name="name">音频资源名称</param>
        /// <param name="onComplete">播放完成后的回调，注意：一次性音频的完成回调依赖于音频长度估计，可能不够准确</param>
        /// <returns>是否成功播放</returns>
        public bool PlayOneShot(string name, Action onComplete = null)
        {
            if (!CheckInitialization()) return false;

            AudioClip clip = GetAudioClip(name);
            if (clip == null) return false;

            mAudioSource.PlayOneShot(clip);
            Debug.Log($"正在播放一次性音频: {name}");

            // 对于PlayOneShot，我们使用延时回调
            if (onComplete != null)
            {
                StartCoroutine(OneShotCompletionDelay(clip.length, onComplete));
            }

            return true;
        }

        /// <summary>
        /// 直接播放音频剪辑
        /// </summary>
        /// <param name="clip">要播放的音频剪辑</param>
        /// <param name="onComplete">播放完成后的回调，注意：一次性音频的完成回调依赖于音频长度估计，可能不够准确</param>
        /// <returns>是否成功播放</returns>
        public bool PlayOneShot(AudioClip clip, Action onComplete = null)
        {
            if (!CheckInitialization() || clip == null) return false;

            mAudioSource.PlayOneShot(clip);
            Debug.Log($"正在播放一次性音频剪辑: {clip.name}");

            // 对于PlayOneShot，我们使用延时回调
            if (onComplete != null)
            {
                StartCoroutine(OneShotCompletionDelay(clip.length, onComplete));
            }

            return true;
        }

        /// <summary>
        /// 停止当前音频的播放
        /// </summary>
        public void StopSound()
        {
            if (!CheckInitialization()) return;

            StopCheckCompletionCoroutine();
            onAudioCompleted = null;

            if (mAudioSource.isPlaying)
            {
                mAudioSource.Stop();
                Debug.Log("音频播放已停止");
            }
        }

        /// <summary>
        /// 暂停当前音频的播放
        /// </summary>
        public void PauseSound()
        {
            if (!CheckInitialization()) return;

            if (mAudioSource.isPlaying)
            {
                mAudioSource.Pause();
                Debug.Log("音频播放已暂停");
            }
        }

        /// <summary>
        /// 恢复已暂停的音频播放
        /// </summary>
        public void ResumeSound()
        {
            if (!CheckInitialization()) return;

            if (!mAudioSource.isPlaying && mAudioSource.clip != null)
            {
                mAudioSource.UnPause();
                Debug.Log("音频播放已恢复");
            }
        }

        /// <summary>
        /// 设置音量大小
        /// </summary>
        /// <param name="volume">音量值 (0.0 - 1.0)</param>
        public void SetVolume(float volume)
        {
            if (!CheckInitialization()) return;

            // 限制音量在合理范围内
            volume = Mathf.Clamp01(volume);
            mAudioSource.volume = volume;
            Debug.Log($"音量已设置为: {volume}");
        }

        /// <summary>
        /// 获取当前音量
        /// </summary>
        /// <returns>当前音量值</returns>
        public float GetVolume()
        {
            return CheckInitialization() ? mAudioSource.volume : 0f;
        }

        /// <summary>
        /// 淡入淡出效果（需要在MonoBehaviour中通过StartCoroutine调用）
        /// </summary>
        /// <param name="targetVolume">目标音量</param>
        /// <param name="duration">过渡时长（秒）</param>
        /// <returns>协程迭代器</returns>
        public IEnumerator FadeVolume(float targetVolume, float duration)
        {
            if (!CheckInitialization()) yield break;

            float startVolume = mAudioSource.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                mAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
                yield return null;
            }

            mAudioSource.volume = targetVolume;
            Debug.Log($"音量已淡入淡出到: {targetVolume}");
        }

        /// <summary>
        /// 获取音频剪辑，优先从缓存获取
        /// </summary>
        /// <param name="name">音频资源名称</param>
        /// <returns>音频剪辑</returns>
        private AudioClip GetAudioClip(string name)
        {
            // 检查缓存中是否已有此音频
            if (audioCache.TryGetValue(name, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            // 从Resources加载音频
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{name}");
            if (clip == null)
            {
                Debug.LogWarning($"无法加载音频资源: Audio/{name}");
                return null;
            }

            // 添加到缓存
            audioCache[name] = clip;
            return clip;
        }

        /// <summary>
        /// 检查音频管理器是否已初始化
        /// </summary>
        /// <returns>是否已正确初始化</returns>
        private bool CheckInitialization()
        {
            if (!isInitialized || mAudioSource == null)
            {
                Debug.LogError("音频管理器未正确初始化或AudioSource已销毁");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检测音频是否播放完成的协程
        /// </summary>
        private IEnumerator CheckAudioCompletion()
        {
            // 等待直到音频停止播放或音频片段变为null
            while (mAudioSource != null && mAudioSource.isPlaying)
            {
                yield return null;
            }

            // 确保不是因为被停止而退出循环
            if (mAudioSource != null && onAudioCompleted != null)
            {
                Action callback = onAudioCompleted;
                onAudioCompleted = null;  // 清空回调，防止多次调用
                callback.Invoke();
                Debug.Log("音频播放完成，已执行回调");
            }

            checkCompletionCoroutine = null;
        }

        /// <summary>
        /// 为PlayOneShot方法提供延时回调的协程
        /// </summary>
        /// <param name="clipLength">音频片段长度</param>
        /// <param name="onComplete">完成回调</param>
        private IEnumerator OneShotCompletionDelay(float clipLength, Action onComplete)
        {
            yield return new WaitForSeconds(clipLength);
            onComplete?.Invoke();
            Debug.Log("一次性音频播放完成，已执行回调");
        }

        /// <summary>
        /// 停止检测音频完成的协程
        /// </summary>
        private void StopCheckCompletionCoroutine()
        {
            if (checkCompletionCoroutine != null)
            {
                StopCoroutine(checkCompletionCoroutine);
                checkCompletionCoroutine = null;
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            StopCheckCompletionCoroutine();
            onAudioCompleted = null;

            if (mAudioSource != null && mAudioSource.isPlaying)
            {
                mAudioSource.Stop();
            }

            // 清空缓存
            if (audioCache != null)
            {
                audioCache.Clear();
            }

            mAudioSource = null;
            isInitialized = false;
            Debug.Log("音频管理器已销毁");
        }

        /// <summary>
        /// 兼容旧版API的方法
        /// </summary>
        #region 兼容旧版API

        // 为保持与原有代码的兼容性，保留以下方法

        /// <summary>
        /// 播放音频（兼容原有API）
        /// </summary>
        /// <param name="name">音频名称</param>
        /// <param name="loop">是否循环</param>
        /// <param name="onComplete">播放完成后的回调</param>
        public void AudioPlay(string name, bool loop = false, Action onComplete = null)
        {
            PlaySound(name, loop, onComplete);
        }

        /// <summary>
        /// 播放一次性音频（兼容原有API）
        /// </summary>
        /// <param name="name">音频名称</param>
        public void AudioPlayOneShot(string name)
        {
            PlayOneShot(name);
        }

        /// <summary>
        /// 停止音频（兼容原有API）
        /// </summary>
        public void AudioStop()
        {
            StopSound();
            mAudioSource.clip = null;
        }

        #endregion
    }
}