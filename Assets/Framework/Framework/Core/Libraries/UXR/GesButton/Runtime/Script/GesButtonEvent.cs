using Rokid.UXR.Interaction;
using UnityEngine;
namespace XY.UXR.Gesture.Button
{
    public class DownwardEvent
    {
        public int handCount;
        public Vector3 startPoint;
        public Vector3 endPoint;
    }
    public class GesButtonEvent : GestureBase
    {
        [Header("collider预制")]
        [SerializeField]
        public GameObject m_collider;
        private GestureBean leftBean = null;
        private GestureBean rightBean = null;
        private FingerEvent[] mCollider = new FingerEvent[2];

        private SkeletonIndexFlag skeletonIndexFlag = SkeletonIndexFlag.INDEX_FINGER_TIP;
        // private SkeletonIndexFlag skeletonIndexFlag = SkeletonIndexFlag.MIDDLE_FINGER_MCP;


        private void Awake()
        {
            mCollider[0] = Instantiate(m_collider).GetComponent<FingerEvent>();
            mCollider[0].name = "leftcollider";
            mCollider[0].gameObject.SetActive(false);
            mCollider[1] = Instantiate(m_collider).GetComponent<FingerEvent>();
            mCollider[1].name = "rightcollider";
            mCollider[1].gameObject.SetActive(false);
            BindEvent();
        }
        public void SetSkeletonIndexFlag(SkeletonIndexFlag flag)
        {
            skeletonIndexFlag = flag;
        }
        private void OnDestroy()
        {
            UnBindEvent();
            foreach (FingerEvent item in mCollider)
            {
                if (item != null)
                {
                    Destroy(item?.gameObject);
                }
            }
        }
        public override void OnTrackedSuccess(HandType handType)
        {
            base.OnTrackedSuccess(handType);
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
        public override void OnTrackedFailed(HandType handType)
        {
            base.OnTrackedFailed(handType);

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
        public override void OnRenderHand(HandType handType, GestureBean gestureBean)
        {
            base.OnRenderHand(handType, gestureBean);

            if (handType == HandType.LeftHand)
            {
                leftBean = gestureBean;
            }
            if (handType == HandType.RightHand)
            {
                rightBean = gestureBean;
            }
        }
        int handCount = 0;
        private void Update()
        {
            if (leftBean != null)
            {
                Vector3 forefinger = leftBean.skeletons[(int)skeletonIndexFlag];
                mCollider[0].transform.position = forefinger;
                mCollider[0].SetHandCount(rightBean != null ? 2 : 1);
            }
            if (rightBean != null)
            {
                Vector3 forefinger = rightBean.skeletons[(int)skeletonIndexFlag];
                mCollider[1].transform.position = forefinger;
                mCollider[1].SetHandCount(leftBean != null ? 2 : 1);
            }
        }
    }
}