using UnityEngine;

public class FollowItem : MonoBehaviour
{
    [Header("��Ҫ�����Ŀ�� Ĭ�������")]
    public Transform target = null;
    [Header("�Ƿ�ˮƽ����")]
    public bool horizontal = true;
    [Header("�Ƿ�ת")]
    public bool flip = false;
    [Header("ֱ�ӿ���׷��")]
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