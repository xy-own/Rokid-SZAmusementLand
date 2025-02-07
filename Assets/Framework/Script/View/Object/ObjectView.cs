using UnityEngine;
public class ObjectView : BaseBehaviour, IObjectView
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
        Destroy(gameObject);
    }
}