using UnityEngine;
using System.Collections;

public class SoundManager : BaseManager
{
    private Hashtable sounds = new Hashtable();
    private AudioSource mAudios = null;
    public override void Initialize()
    {
        if (!ManagementCenter.managerObject.TryGetComponent(out mAudios))
        {
            mAudios = ManagementCenter.managerObject.AddComponent<AudioSource>();
            mAudios.playOnAwake = false;
            mAudios.loop = false;
        }
        LoadAudioData();
    }
    // 音效
    public void AudioPlay(string name)
    {
        AudioClip ac = Get(name);
        mAudios.PlayOneShot(ac);
    }

    public void SoundPlay(string name, bool loop = true)
    {
        AudioClip ac = Get(name);
        SoundStop();
        mAudios.loop = loop;
        if (mAudios.clip==null || mAudios.clip.GetHashCode() != ac.GetHashCode())
        {
            mAudios.clip = ac;
        }
        mAudios.Play();
    }
    // Close BGM
    public void SoundStop()
    {
        if (mAudios.isPlaying)
        {
            mAudios.Stop();
        }
    }
    AudioClip Get(string key)
    {
        if (sounds[key] == null)
        {
            Util.Log("没找到该音效", LogColor.Red);
            return null;
        }
        return sounds[key] as AudioClip;
    }
    void Add(string key, AudioClip value)
    {
        if (sounds[key] != null || value == null)
        {
            Util.Log("音效文件错误", LogColor.Red);
            return;
        }
        sounds.Add(key, value);
    }
    void LoadAudioData()
    {
        TextAsset tAssets = Resources.Load<TextAsset>("Audio/config");
        if (tAssets == null)
        {
            Util.Log("没有该资源");
            return;
        }
        string txt = tAssets.text;
        if (txt != string.Empty)
        {
            AssetConfig info = JsonUtility.FromJson<AssetConfig>(txt);
            for (int i = 0; i < info.name.Count; i++)
            {
                string objName = info.name[i];
                AudioClip obj = Resources.Load<AudioClip>($"Audio/{objName}");
                if (obj == null)
                {
                    Util.Log("Load Audio Error", LogColor.Red);
                }
                Add(objName, obj);
            }
        }
    }
    public override void OnUpdate(float deltaTime)
    {
    }
    public override void OnDispose()
    {
    }
}