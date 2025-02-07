using System;
using System.Collections;
using UnityEngine;

public class ObjTrigger : MonoBehaviour
{
    public Action<Collider> enterAction;
    public Action<Collider> exitAction;

    public void Enter(Collider go)
    {
        enterAction?.Invoke(go);
    }
    public void Exit(Collider go)
    {
        exitAction?.Invoke(go);
    }
}
