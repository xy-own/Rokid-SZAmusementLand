using System;
using Rokid.UXR.Interaction;
using UnityEngine;
namespace D
{
    public class GesButtonEvent : MonoBehaviour
    {
        [Header("collider预制")]
        [SerializeField]
        public GameObject m_collider;
        private GestureBean leftBean = null;
        private GestureBean rightBean = null;
        private FingerEvent[] mCollider = new FingerEvent[2];

        [SerializeField]
        private SkeletonIndexFlag skeletonIndexFlag = SkeletonIndexFlag.INDEX_FINGER_TIP;

        private void Awake()
        {
            m_collider = Resources.Load<GameObject>("Forefinger");
            mCollider[0] = Instantiate(m_collider).AddComponent<FingerEvent>();
            mCollider[0].name = "leftcollider";
            mCollider[0].gameObject.SetActive(false);
            mCollider[1] = Instantiate(m_collider).AddComponent<FingerEvent>();
            mCollider[1].name = "rightcollider";
            mCollider[1].gameObject.SetActive(false);
            GesEventInput.OnTrackedSuccess += OnTrackedSuccess;
            GesEventInput.OnTrackedFailed += OnTrackedFailed;
            GesEventInput.OnRenderHand += OnRenderHand;
        }
        private void OnDestroy()
        {
            GesEventInput.OnTrackedSuccess -= OnTrackedSuccess;
            GesEventInput.OnTrackedFailed -= OnTrackedFailed;
            GesEventInput.OnRenderHand -= OnRenderHand;
            foreach (FingerEvent item in mCollider)
            {
                if (item != null)
                {
                    Destroy(item?.gameObject);
                }
            }
        }
        void OnTrackedSuccess(HandType handType)
        {
            if (handType == HandType.None)
            {
                mCollider[0].gameObject.SetActive(true);
                mCollider[1].gameObject.SetActive(true);
            }
            if (handType == HandType.LeftHand)
            {
                mCollider[0].gameObject.SetActive(true);
            }
            if (handType == HandType.RightHand)
            {
                mCollider[1].gameObject.SetActive(true);
            }

        }
        void OnTrackedFailed(HandType handType)
        {
            if (handType == HandType.None)
            {
                leftBean = null;
                rightBean = null;
                mCollider[0].gameObject.SetActive(false);
                mCollider[1].gameObject.SetActive(false);
            }
            if (handType == HandType.LeftHand)
            {
                leftBean = null;
                mCollider[0].gameObject.SetActive(false);
            }
            if (handType == HandType.RightHand)
            {
                rightBean = null;
                mCollider[1].gameObject.SetActive(false);
            }
        }
        void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
            }
        }
        private void Update()
        {
            if (leftBean != null)
            {
                Vector3 forefinger = leftBean.skeletons[(int)skeletonIndexFlag];
                mCollider[0].transform.position = forefinger;
            }
            if (rightBean != null)
            {
                Vector3 forefinger = rightBean.skeletons[(int)skeletonIndexFlag];
                mCollider[1].transform.position = forefinger;
            }
        }
    }
}