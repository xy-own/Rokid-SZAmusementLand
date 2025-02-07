using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SDKManager : BaseManager
{
    public override void Initialize()
    {
        Util.Log("SDKManager >>>>> Initialize");

        MessageDispatcher.AddListener<GameObject>(FovOpenAPI.AddFov, AddFovItem);
        MessageDispatcher.AddListener<GameObject>(FovOpenAPI.RemoveFov, RemoveFovItem);

        // isOnUpdate = true;
    }
    float objectEffectiveDistance = 4f;
    public override void OnUpdate(float deltaTime)
    {
        foreach (FovDetection item in areaList.Values)
        {
            Vector3 worldToViewportPoint = Main.gameCamera.WorldToViewportPoint(item.transform.position);
            bool visible = Vector3.Distance(item.transform.position, Main.gameCamera.transform.position) < objectEffectiveDistance;

            if (worldToViewportPoint.z < 0)
            {
                visible = false;
            }
            if (!(worldToViewportPoint.x > 0 && worldToViewportPoint.x < 1))
            {
                visible = false;
            }
            //if (!(worldToViewportPoint.y > 0 && worldToViewportPoint.y < 1))
            //{
            //    visible = false;
            //}

            item.Detection(visible);
        }
    }
    public override void OnDispose()
    {
        MessageDispatcher.RemoveListener<GameObject>(FovOpenAPI.AddFov, AddFovItem);
        MessageDispatcher.RemoveListener<GameObject>(FovOpenAPI.RemoveFov, RemoveFovItem);
    }

    private Dictionary<string, FovDetection> areaList = new Dictionary<string, FovDetection>();
    void AddFovItem(GameObject go)
    {
        if (!areaList.ContainsKey(go.name))
        {
            areaList.Add(go.name, go.GetComponent<FovDetection>());
        }
    }
    void RemoveFovItem(GameObject go)
    {
        if (areaList.ContainsKey(go.name))
        {
            areaList.Remove(go.name);
        }
    }


}