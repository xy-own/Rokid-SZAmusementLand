using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GManager : BaseManager
{
    public override void Initialize()
    {
        Util.Log("GManager >>>>> Initialize");
        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = AppConst.GameFrameRate;
        DOTween.Init(true, true, LogBehaviour.Default);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitializeOK();
    }

    void InitializeOK()
    {
        if (AppConst.ShowFps)
        {
            ManagementCenter.main.gameObject.AddComponent<AppFPS>();
        }
#if UNITY_EDITOR
        ManagementCenter.main.gameObject.AddComponent<GameCMD>();
#endif
        Main.configMgr.Initialize();
        Main.sdkMgr.Initialize();
        Main.effectMgr.Initialize();
        Main.soundMgr.Initialize();
        Main.uiMgr.Initialize();
        Main.logicManager.Initialize();

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        Rokid.UXR.Interaction.GesEventInput.Instance.Interactor.Find("LeftHandInteractors").gameObject.SetActive(false);
        Rokid.UXR.Interaction.GesEventInput.Instance.Interactor.Find("RightHandInteractors").gameObject.SetActive(false);
        //GameObject gameMainView = Instantiate(Main.configMgr.GetGameObject("ObjectMainView"));
        //gameMainView.transform.SetParent(ManagementCenter.main.transform, false);
        //gameMainView.AddComponent<ViewObject>().BindView(new MainObjectView());
    }

    public override void OnUpdate(float deltaTime)
    {
    }
    public override void OnDispose()
    {
        Main.logicManager.OnDispose();
        Main.uiMgr.OnDispose();
        Main.soundMgr.OnDispose();
        Main.effectMgr.OnDispose();
        Main.sdkMgr.OnDispose();
        Main.configMgr.OnDispose();
        Util.Log("~GameManager was destroyed");
    }
}