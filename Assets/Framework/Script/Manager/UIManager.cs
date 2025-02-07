using UnityEngine;
using System.Collections.Generic;
public enum PanelType
{
    MainView,
}
public class UIManager : BaseManager
{
    Stack<ViewObject> uiStack = new Stack<ViewObject>();
    Dictionary<string, ViewObject> viewList = new Dictionary<string, ViewObject>();
    public override void Initialize()
    {
        Debug.Log("UI >>> Initialize");
    }
    public ViewObject ShowView(PanelType type, bool peekVisible = false)
    {
        string viewName = type.ToString();
        ViewObject view = null;
        viewList.TryGetValue(viewName, out view);
        if (view == null)
        {
            GameObject uiView;
            uiView = LoadUIPanel(viewName);
            uiView.SetActive(false);
            view = uiView.AddComponent<ViewObject>();
            switch (type)
            {
                case PanelType.MainView:
                    view.BindView(new MainView());
                    break;

            }
            viewList.Add(viewName, view);
        }
        PushPanel(view, peekVisible);
        return view;
    }
    GameObject LoadUIPanel(string name)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>($"UI/{name}"));
        if (go == null)
        {
            Debug.Log("未找到面板");
        }
        go.name = name;
        go.transform.SetParent(Main.uiCanvas.transform,false);
        return go;
    }
    public void ClearView()
    {
        uiStack.Clear();
        if (viewList.Count > 0)
        {
            foreach (ViewObject panel in viewList.Values)
            {
                Destroy(panel.gameObject);
            }
            viewList.Clear();
        }
        Util.ClearMemory();
        Debug.Log($"ui堆栈数量 : {uiStack.Count}");
    }
    public ViewObject PeekPanel()
    {
        return uiStack.Peek();
    }
    public void PushPanel(ViewObject panel, bool peekVisible)
    {
        if (uiStack.Count > 0)
        {
            PeekPanel().GetComponent<CanvasGroup>().blocksRaycasts = false;
            PeekPanel().gameObject.SetActive(peekVisible);
        }
        uiStack.Push(panel);
        PeekPanel().GetComponent<CanvasGroup>().blocksRaycasts = true;
        PeekPanel().gameObject.SetActive(true);
        Debug.Log($"ui堆栈数量 : {uiStack.Count}");
    }
    public void PopPanel()
    {
        PeekPanel().GetComponent<CanvasGroup>().blocksRaycasts = false;
        PeekPanel().gameObject.SetActive(false);
        uiStack.Pop();
        if (uiStack.Count > 0)
        {
            PeekPanel().GetComponent<CanvasGroup>().blocksRaycasts = true;
            PeekPanel().gameObject.SetActive(true);
        }
        Debug.Log($"ui堆栈数量 : {uiStack.Count}");
    }
    public override void OnUpdate(float deltaTime)
    {

    }
    public override void OnDispose()
    {
        Debug.Log("~UIManager was destroyed");
    }
}