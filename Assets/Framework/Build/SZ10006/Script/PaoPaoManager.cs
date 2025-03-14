using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture.Button;

public class PaoPaoManager : MonoBehaviour
{
    private GameObject m_PaoPaoPo;
    private GameObject m_PaoPao;
    // Start is called before the first frame update
    void Start()
    {
        m_PaoPaoPo = transform.Find("effect_paopao_bao").gameObject;
        m_PaoPao = transform.Find("effect_paopao_body").gameObject;
        transform.DOLocalMoveY(2.8f, 2f);
        gameObject.AddComponent<BtnItem>().enterAction += ClickPaoPao;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClickPaoPao(FingerEvent fingerEvent,Collider go)
    {
        MessageDispatcher.SendMessageData("10006AudioShot", "ChuPengQiPao");
        m_PaoPao.SetActive(false);
        m_PaoPaoPo.SetActive(true);
    }
}
