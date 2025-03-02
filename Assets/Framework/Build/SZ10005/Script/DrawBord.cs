using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using XY.UXR.Gesture.Button;

/// <summary>
/// 画板
/// </summary>
public class DrawBord : MonoBehaviour
{
    // 当画笔移动速度很快时，为了不出现断断续续的点，所以需要对两个点之间进行插值，lerp 就是插值系数
    [Range(0, 1)]
    public float lerp = 0.05f;
    // 初始化背景的图片
    public Texture2D initailizeTexture;
    // 当前背景的图片
    private Texture2D currentTexture;
    // 画笔所在位置映射到画板图片的 UV 坐标
    private Vector2 paintPos;

    private bool isDrawing = false; // 当前画笔是不是正在画板上
    // 离开时画笔所在的位置 
    private int lastPaintX;
    private int lastPaintY;
    // 画笔所代表的色块的大小
    private int painterTipsWidth = 30;
    private int painterTipsHeight = 15;
    // 当前画板的背景图片的尺寸
    private int textureWidth;
    private int textureHeight;

    // 画笔的颜色
    private Color32[] painterColor;

    private Color32[] currentColor;
    private Color32[] originColor;

    // 触发绘画的物体
    private Transform drawingObject;

    //当前画板的绘制颜色
    private Color m_CurrentBrushColor;

    public GameObject m_CanCollider;

    private int m_Num;

    public List<GameObject> m_Collider;

    private void Start()
    {
        // 获取原始图片的大小 
        Texture2D originTexture = GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        textureWidth = originTexture.width; // 1920   
        textureHeight = originTexture.height; // 1080

        // 设置当前图片
        currentTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);
        currentTexture.SetPixels32(originTexture.GetPixels32());
        currentTexture.Apply();

        // 赋值给黑板
        GetComponent<MeshRenderer>().material.mainTexture = currentTexture;

        // 初始化画笔的颜色
        painterColor = Enumerable.Repeat<Color32>(new Color32(181, 255, 255, 255), painterTipsWidth * painterTipsHeight).ToArray<Color32>();

        // 将该物体的碰撞体设置为触发类型
        Collider collider = transform.Find("Sphere").GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        BtnItem triggerEvent = m_CanCollider.GetComponent<BtnItem>();
        triggerEvent.enterAction += TriggerEnter;
        triggerEvent.exitAction += TriggerExit;

        for (int i = 0;i < m_Collider.Count;i++)
        {
            m_Collider[i].AddComponent<BtnItem>().enterAction += FingerEnter;
            m_Collider[i].SetActive(true);
            //if (i > 0)
            //{
            //    m_Collider[i].SetActive(false);
            //}
        }
    }

    private void LateUpdate()
    {
        if (isDrawing && drawingObject != null)
        {
            // 获取物体在世界空间中的位置
            Vector3 objectPos = drawingObject.position;

            // 将世界空间位置映射到 UV 坐标
            Vector3 localPos = transform.InverseTransformPoint(objectPos);

            // 调整 X 轴和 Y 轴方向
            paintPos.x = (localPos.x + 0.5f);
            paintPos.y = (localPos.y + 0.5f); // 反转 Y 轴方向

            // 确保 UV 坐标在 [0, 1] 范围内
            paintPos.x = Mathf.Clamp01(paintPos.x);
            paintPos.y = Mathf.Clamp01(paintPos.y);
        }

        // 计算当前画笔，所代表的色块的一个起始点
        int texPosX = (int)(paintPos.x * (float)textureWidth - (float)(painterTipsWidth / 2));
        int texPosY = (int)(paintPos.y * (float)textureHeight - (float)(painterTipsHeight / 2));

        // 边界检查
        texPosX = Mathf.Clamp(texPosX, 0, textureWidth - painterTipsWidth);
        texPosY = Mathf.Clamp(texPosY, 0, textureHeight - painterTipsHeight);

        if (isDrawing)
        {
            // 改变画笔所在的块的像素值
            currentTexture.SetPixels32(texPosX, texPosY, painterTipsWidth, painterTipsHeight, painterColor);
            // 如果快速移动画笔的话，会出现断续的现象，所以要插值
            if (lastPaintX != 0 && lastPaintY != 0)
            {
                int lerpCount = (int)(1 / lerp);
                for (int i = 0; i <= lerpCount; i++)
                {
                    int x = (int)Mathf.Lerp((float)lastPaintX, (float)texPosX, lerp * i);
                    int y = (int)Mathf.Lerp((float)lastPaintY, (float)texPosY, lerp * i);

                    // 边界检查
                    x = Mathf.Clamp(x, 0, textureWidth - painterTipsWidth);
                    y = Mathf.Clamp(y, 0, textureHeight - painterTipsHeight);

                    currentTexture.SetPixels32(x, y, painterTipsWidth, painterTipsHeight, painterColor);
                }
            }
            currentTexture.Apply();
            lastPaintX = texPosX;
            lastPaintY = texPosY;
        }
        else
        {
            lastPaintX = lastPaintY = 0;
        }
    }

    /// <summary>
    /// 当物体进入触发区时开始绘画
    /// </summary>
    /// <param name="other"></param>
    private void TriggerEnter(FingerEvent fingerEvent, Collider other)
    {
        isDrawing = true;
        drawingObject = fingerEvent.gameObject.transform;
    }

    /// <summary>
    /// 当物体离开触发区时停止绘画
    /// </summary>
    /// <param name="other"></param>
    private void TriggerExit(FingerEvent fingerEvent, Collider other)
    {
        //if (fingerEvent.gameObject.transform == drawingObject)
        //{
        //    isDrawing = false;
        //    drawingObject = null;
        //    if (m_Num >8)
        //    {
        //        MessageDispatcher.SendMessageData("GetDrawCom");
        //    }
        //    else
        //    {
        //        m_Num = 0;
        //        // 设置当前图片
        //        currentTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);
        //        currentTexture.SetPixels32(initailizeTexture.GetPixels32());
        //        currentTexture.Apply();

        //        // 赋值给黑板
        //        GetComponent<MeshRenderer>().material.mainTexture = currentTexture;

        //        for (int i = 0; i < m_Collider.Count; i++)
        //        {
        //            m_Collider[i].gameObject.SetActive(true);
        //            MessageDispatcher.SendMessageData("DrawFail");
        //        }
        //    }
        //}
    }


    private void FingerEnter(FingerEvent fingerEvent,Collider go)
    {
        m_Num++;
        m_Collider[m_Num].SetActive(true);
        go.gameObject.SetActive(false);
        MessageDispatcher.SendMessageData("10005AudioShot", "shouzhan");
        if (m_Num>= 8)
        {
            currentTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);
            currentTexture.SetPixels32(initailizeTexture.GetPixels32());
            currentTexture.Apply();
            GetComponent<MeshRenderer>().material.mainTexture = currentTexture;
            MessageDispatcher.SendMessageData("GetDrawCom");
            isDrawing = false;
            drawingObject = null;
        }
    }
}