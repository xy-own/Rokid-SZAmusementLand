using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TemplateConfig", menuName = "ScriptableObjects/TemplateConfig", order = 1)]
public class TemplateConfig : ScriptableObject
{
    public List<TemplateInfo> templates;

    [System.Serializable]
    public class TemplateInfo
    {
        public string name;
        public TextAsset templateFile;
        public Texture2D icon;
    }
}
