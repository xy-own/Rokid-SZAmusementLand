using UnityEngine;
using UnityEngine.UI;
public class MainView : UIView
{
    public override void OnAwake()
    {
    }
    public override void OnStart()
    {
        Debug.Log("OnStart");
        GameObject go = gameObject.transform.Find("Button").gameObject;
        BindEvent(go, ClickEvent);
    }
    private void ClickEvent(GameObject go)
    {
        Util.Log($" ------> click {go.name}");
    }
    public override void OnUpdate()
    {
    }
    public override void OnDispose()
    {
    }
}