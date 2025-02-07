using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LogicManager : BaseManager
{
    public override void Initialize()
    {
        Util.Log("LogicManager >>>>> Initialize");
        Main.uiMgr.ShowView(PanelType.MainView);
    }
    public override void OnUpdate(float deltaTime)
    {
    }
    public override void OnDispose()
    {
        Util.Log("LoginManager OnDispose");
    }
}