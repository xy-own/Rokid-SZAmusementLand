using UnityEngine;
using UnityEngine.Events;
public class UIView : BaseBehaviour, IUIView
{
    public GameObject gameObject;
    public ViewObject viewObject;

    public virtual void OnAwake()
    {
    }
    public virtual void OnStart()
    {
    }
    public virtual void OnUpdate()  
    {
    }
    public virtual void OnDispose()
    {
    }

    protected void BindEvent(GameObject go, UnityAction<GameObject> act)
    {
        EventTriggerListener.Get(go).onClick = (clickItem) => { act(clickItem); };
    }
}