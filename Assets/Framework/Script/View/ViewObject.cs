using System;
using UnityEngine;

public class ViewObject : MonoBehaviour
{
    private IUIView uiView;
    private IObjectView objView;

    private void Awake()
    {
        if (uiView != null)
        {
            uiView.OnAwake();
        }
        if (objView != null)
        {
            objView.OnAwake();
        }
    }
    private void Start()
    {
        if (uiView != null)
        {
            uiView.OnStart();
        }
        if (objView != null)
        {
            objView.OnStart();
        }
    }
    private void Update()
    {
        if (uiView != null)
        {
            uiView.OnUpdate();
        }
        if (objView != null)
        {
            objView.OnUpdate();
        }
    }
    private void OnDestroy()
    {
        if (uiView != null)
        {
            uiView.OnDispose();
        }
        if (objView != null)
        {
            objView.OnDispose();
        }
    }
    public void BindView(IUIView view)
    {
        uiView = view;
        var _npcView = view as UIView;
        if (_npcView != null)
        {
            _npcView.viewObject = this;
            _npcView.gameObject = gameObject;
        }
    }
    public void BindView(IObjectView view)
    {
        objView = view;
        var _objView = objView as ObjectView;
        _objView.viewObject = this;
        _objView.gameObject = gameObject;
    }
}