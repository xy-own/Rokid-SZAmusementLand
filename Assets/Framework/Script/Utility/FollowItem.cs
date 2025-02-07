using UnityEngine;

public class FollowItem : MonoBehaviour
{
    [Header("需要朝向的目标 默认摄像机")]
    public Transform target = null;
    [Header("是否水平朝向")]
    public bool horizontal = true;
    [Header("是否翻转")]
    public bool flip = false;
    [Header("直接开启追踪")]
    public bool onAwake = true;

    private bool move = false;

    private void Awake()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
        if (onAwake)
        {
            TrackingStatus(true);
        }
    }
    public void TrackingStatus(bool status)
    {
        move = status;
    }
    private void FixedUpdate()
    {
        if (move)
        {
            Vector3 pos = target.position;
            if (horizontal)
            {
                pos.y = transform.position.y;
            }
            Vector3 dir = (transform.position - pos).normalized;
            if (flip)
            {
                dir *= -1;
            }
            transform.forward = dir;
        }
    }
    public void UpdateOne()
    {
        Vector3 pos = target.position;
        if (horizontal)
        {
            pos.y = transform.position.y;
        }
        Vector3 dir = (transform.position - pos).normalized;
        if (flip)
        {
            dir *= -1;
        }
        transform.forward = dir;
    }

#if UNITY_EDITOR
    [InspectorButton("ChangeTrackingStatus")]
    public bool mTracking;
    private void ChangeTrackingStatus()
    {
        TrackingStatus(!move);
    }

    [InspectorButton("HorizontalStatus")]
    public bool mHorizontal;
    private void HorizontalStatus()
    {
        horizontal = !horizontal;
    }


    [InspectorButton("FlipStatus")]
    public bool mFlip;
    private void FlipStatus()
    {
        flip = !flip;
    }
#endif
}