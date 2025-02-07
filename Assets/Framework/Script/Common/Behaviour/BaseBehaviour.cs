using System.Collections;

public abstract class BaseBehaviour
{
    public T Instantiate<T>(T original) where T : UnityEngine.Object
    {
        return UnityEngine.GameObject.Instantiate<T>(original);
    }

    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            UnityEngine.GameObject.Destroy(obj);
        }
    }

    public static void Destroy(UnityEngine.Object obj, float t)
    {
        if (obj != null)
        {
            UnityEngine.GameObject.Destroy(obj, t);
        }
    }

    public UnityEngine.Coroutine StartCoroutine(IEnumerator routine)
    {
        return ManagementCenter.main.StartCoroutine(routine);
    }
    public void StopCoroutine(IEnumerator routine)
    {
        ManagementCenter.main.StopCoroutine(routine);
    }
}
