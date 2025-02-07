using UnityEngine;
using UnityEngine.Events;


public class FovOpenAPI
{
    public const string AddFov = "AddFov";
    public const string RemoveFov = "RemoveFov";
}
public class FovDetection : MonoBehaviour
{
    private bool visible = false;
    public UnityAction<GameObject, bool> action;

    private void OnEnable()
    {
        MessageDispatcher.SendMessageData(FovOpenAPI.AddFov, gameObject);
    }

    private void OnDisable()
    {
        MessageDispatcher.SendMessageData(FovOpenAPI.RemoveFov, gameObject);
    }
    public void Detection(bool visible)
    {
        if (this.visible == visible)
            return;
        this.visible = visible;
        action?.Invoke(gameObject, this.visible);
    }
}
