using UnityEngine;

public class SlamTools
{
    private GameObject mItem;
    private Vector3 oldPos = Vector3.zero;

    /// <summary>
    /// 绑定游戏对象
    /// </summary>
    /// <param name="item"> 需要移动和重置的游戏对象 </param>
    public void Init(GameObject item)
    {
        mItem = item;
        oldPos = mItem.transform.position;
    }
    /// <summary>
    /// 需要移动的位置
    /// </summary>
    /// <param name="worldPosition"> </param>
    /// <param name="action"> 移动事件完成的回调 </param>
    public void Move(Vector3 worldPosition,Vector3 worldRotation)
    {
        Vector3 endPos = worldPosition;
        endPos.y = oldPos.y;

        mItem.transform.position = endPos;
        mItem.transform.eulerAngles = new Vector3(0f, worldRotation.y, 0f);
    }
    /// <summary>
    /// 移回原位
    /// </summary>
    /// <param name="action"> 移动事件完成的回调 </param>
    public void ResetPosition()
    {
        mItem.transform.position = oldPos;
        mItem.transform.forward = Vector3.forward;
    }
}
