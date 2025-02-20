using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZ10004
{
    public class AudioManager : MonoBehaviour
    {
        public AudioManager(AudioSource source)
        {
            mAudioSource = source;
        }
        private AudioSource mAudioSource = null;

        public void AudioPlay(string name, bool loop = false)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{name}");
            mAudioSource.loop = loop;
            if (clip == null)
                return;
            mAudioSource.clip = clip;
            mAudioSource.Play();
        }

        public void AudioPlayOneShot(string name)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{name}");
            if (clip == null)
                return;
            mAudioSource.PlayOneShot(clip);
        }

        public void AudioStop()
        {
            if (mAudioSource.isPlaying)
            {
                mAudioSource.Stop();
                mAudioSource.clip = null;
            }
        }

        public void Dispose()
        {
            AudioStop();
            mAudioSource = null;
        }
    }
}
