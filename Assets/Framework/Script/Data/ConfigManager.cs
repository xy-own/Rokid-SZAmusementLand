using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : BaseManager
{
    private static ConfigManager instance;
    private Dictionary<string, Sprite> mSprite = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> mGameObject = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> mEffect = new Dictionary<string, GameObject>();
    private Dictionary<string, Material> mMaterial = new Dictionary<string, Material>();
    private Dictionary<string, Texture> mTexture = new Dictionary<string, Texture>();

    public static ConfigManager Create()
    {
        if (instance == null)
        {
            instance = new ConfigManager();
        }
        return instance;
    }
    public override void Initialize()
    {
    }
    public override void OnUpdate(float deltaTime)
    {
    }
    public override void OnDispose()
    {
    }
    public GameObject GetGameObject(string name)
    {
        GameObject data = null;
        if (!mGameObject.TryGetValue(name, out data))
        {
            data = Resources.Load<GameObject>($"Prefab/{name}");
            mGameObject.Add(name, data);
        }
        return data;
    }
    public GameObject GetEffect(string name)
    {
        GameObject data = null;
        if (!mEffect.TryGetValue(name, out data))
        {
            data = Resources.Load<GameObject>($"Effect/{name}");
            mEffect.Add(data.name, data);
        }
        return data;
    }
    public Sprite GetSprite(string name)
    {
        Sprite data = null;
        if (!mSprite.TryGetValue(name, out data))
        {
            data = Resources.Load<Sprite>($"Sprite/{name}");
            mSprite.Add(data.name, data);
        }
        return data;
    }
    public Material GetMaterial(string name)
    {
        Material data = null;
        if (!mMaterial.TryGetValue(name, out data))
        {
            data = Resources.Load<Material>($"Material/{name}");
            mMaterial.Add(data.name, data);
        }
        return data;
    }
    public GameObject GetCG(string name)
    {
        return Resources.Load<GameObject>($"Media/{name}");
    }

    public Texture GetTexture(string name)
    {
        Texture data = null;
        if (!mTexture.TryGetValue(name, out data))
        {
            data = Resources.Load<Texture>($"Texture/{name}");
            mTexture.Add(name, data);
        }
        return data;
    }
}