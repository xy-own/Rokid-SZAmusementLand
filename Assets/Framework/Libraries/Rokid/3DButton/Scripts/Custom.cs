using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Rokid.UXR.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XY.UXR
{
    public class Custom : MonoBehaviour
    {
        private Transform bottomCollider;
        private Transform bottom;
        private Transform bottomLight;
        private Transform middle;
        private Transform top;
        //光圈的对象，这个不会动，以此为基线，来做位移和动画
        private Transform bottomInit;


        private AudioSource audioSource;
        public AudioClip ClickClip;
        public AudioClip ElectronicClip;

        private bool isExpand;
        private bool isClicking;
        //游戏对象在世界空间中的实际缩放比例，考虑了父级对象的缩放，这个值在这里是指位移的深度
        private float lossyScale;

        [Header("单次弹出和收起动画的时间")]
        public float animationDuration = 0.1f;
        [Header("食指距离按钮弹出的距离")]
        public float hitDistance = 0.1f;
        [Header("整个按钮弹出时的每层之间的间隙")]
        public float m_expand_layer_height = 0.02f;
        [Header("按钮按压到最底层后需继续下按的距离")]
        public float m_click_depth = -0.03f;
        [Header("按钮初始状态时按钮和底部的间隙")]
        public float m_collapse_layer_height = 0.01f;
        // Click Event
        public UnityEvent onPointerClick;

        private void Awake()
        {
            if (ClickClip != null || ElectronicClip != null)
            {
                if (!TryGetComponent(out audioSource))
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        private void OnEnable()
        {
            lossyScale = transform.lossyScale.y;
            top = transform.Find("Top");
            middle = transform.Find("Middle");
            bottom = transform.Find("Bottom");
            bottomLight = bottom.Find("Light");
            bottomInit = transform.Find("Init");
            bottomCollider = transform.Find("BottomCollider");
            top.localPosition = bottomInit.localPosition + new Vector3(0, 0, 0);
            middle.localPosition =
                bottomInit.localPosition + new Vector3(0, m_collapse_layer_height, 0);
            bottomCollider.localPosition = bottomInit.localPosition + new Vector3(0, m_click_depth, 0);
        }

        public void OnDisable()
        {
            isClicking = false;
            isExpand = false;
            if (bottomInit != null)
            {
                if (top != null)
                {
                    top.localPosition = bottomInit.localPosition + new Vector3(0, 0, 0);
                }

                if (middle != null)
                {
                    middle.localPosition =
                        bottomInit.localPosition + new Vector3(0, m_collapse_layer_height, 0);
                }

                if (bottomCollider != null)
                {
                    bottomCollider.localPosition =
                        bottomInit.localPosition + new Vector3(0, m_click_depth, 0);
                }
            }

            ShowTopOutline(false);
            ChangeButtonAlpha(0);
            ChangeTopAlpha(0);
        }

        public void OnCustomTriggerEnter(Collider other)
        {
            if (!Enable)
            {
                return;
            }

            if (other.gameObject.name == bottomCollider.name)
            {
                isClicking = true;
            }
            else if (other.gameObject.name == bottom.name)
            {
            }
        }

        public void OnCustomTriggerExit(Collider other)
        {
            if (!Enable)
            {
                return;
            }

            if (other.gameObject.name == bottomCollider.name)
            {
            }
        }

        private void start()
        {
            if (!isExpand)
            {
                isExpand = true;
                ExpandAnimation();
            }
        }

        private void end()
        {
            if (isExpand)
            {
                isExpand = false;
                CollapseAnimation();
            }
        }

        private void ExpandAnimation()
        {
            PlayAudio(ElectronicClip);
            var initialPosition = bottomInit.localPosition.y;
            var end = initialPosition + m_expand_layer_height * 2;
            DOTween.To(() => initialPosition, (value =>
            {
                top.localPosition =
                    new Vector3(top.localPosition.x, value, top.localPosition.z);
                var current = 1 - (end - value) / (end - initialPosition);
                ChangeTopAlpha(current);
            }), end, animationDuration).onComplete += () =>
            {
                ShowTopOutline(true);
                isExpand = true;
            };
            middle.DOLocalMoveY(initialPosition + m_expand_layer_height, animationDuration);
        }

        private void CollapseAnimation()
        {
            var start = top.localPosition.y;
            var initialPosition = bottomInit.localPosition.y;
            DOTween.To(() => start, (value =>
            {
                top.localPosition =
                    new Vector3(top.localPosition.x, value, top.localPosition.z);
                var current = (initialPosition - value) / (initialPosition - start);
                ChangeTopAlpha(current);
            }), initialPosition, animationDuration).onComplete += () =>
            {
                ShowTopOutline(false);
                isExpand = false;
            };
            middle.DOLocalMoveY(initialPosition + m_collapse_layer_height, animationDuration);
            bottom.DOLocalMoveY(initialPosition, animationDuration);
        }

        public void onDistanceChange(float hitInfoDistance, HandType handType)
        {
            if (!Enable)
            {
                return;
            }

            if (hitInfoDistance < m_expand_layer_height * 2 * lossyScale + hitDistance)
            {
                //如果小于hitDistance，就触发动画
                Forefinger.Instance.setHandType(handType);
                start();
            }
            else
            {
                if (hitInfoDistance > m_expand_layer_height * 2 * lossyScale + hitDistance * 1.3f)
                {
                    Forefinger.Instance.setHandType(HandType.None);
                    onLoss(0);
                }
            }
        }

        public void onLoss(int type)
        {
            end();
            ChangeButtonAlpha(0);
        }

        private void OnClicked()
        {
            // ChangeTopAlpha(0);
            isClicking = false;
            onPointerClick?.Invoke();
        }

        public void onPress(float hitOneDistance)
        {
            if (!Enable)
            {
                return;
            }

            //todo 可能需要优化为取按钮法线向量的投影长度
            LocalMoveAnimation(hitOneDistance);
            CheckPlayAudio(hitOneDistance);
        }

        private bool Played = false;
        //第一次在范围内不做判断
        private bool m_clicking = false;

        private void CheckPlayAudio(float distance)
        {
            if (distance <= Mathf.Abs(m_click_depth) * lossyScale)
            {
                if (!Played)
                {
                    Played = true;
                    PlayAudio(ClickClip);
                    m_clicking = true;
                }
            }
            else
            {
                if (isClicking && m_clicking)
                {
                    //手收回超过基线就判定为一次点击
                    OnClicked();
                    m_clicking = false;
                }

                Played = false;
            }
        }

        private void LocalMoveAnimation(float distance)
        {
            var initialPosition = bottomCollider.localPosition.y;
            if (distance >= (Mathf.Abs(m_click_depth) + 2 * m_expand_layer_height) * lossyScale)
            {
                //不处理
            }
            else if (distance >= (Mathf.Abs(m_click_depth) + m_expand_layer_height) * lossyScale)
            {
                top.DOLocalMoveY(initialPosition + distance / lossyScale, animationDuration);
            }
            else
            {
                top.DOLocalMoveY(initialPosition + distance / lossyScale, animationDuration);
                middle.DOLocalMoveY(initialPosition + distance / lossyScale + m_collapse_layer_height,
                    animationDuration);
                if (distance <= Mathf.Abs(m_click_depth) * lossyScale)
                {
                    bottom.DOLocalMoveY(1.1f * initialPosition + distance / lossyScale, animationDuration);
                    ChangeButtonAlpha(1);
                }
                else
                {
                    bottom.DOLocalMoveY(bottomInit.localPosition.y, animationDuration);
                }
            }

            if (distance <= (Mathf.Abs(m_click_depth) + 2 * m_expand_layer_height) * lossyScale &&
                distance >= Mathf.Abs(m_click_depth) * lossyScale)
            {
                var alpha = 1 - distance / ((Mathf.Abs(m_click_depth) + 2 * m_expand_layer_height) * lossyScale);
                ChangeButtonAlpha(alpha);
            }
        }

        /**
         *  修改光影和光圈的透明度
         *  <param name="alpha"></param>
         */
        private void ChangeButtonAlpha(float alpha)
        {
            //修改光影的透明度
            if (bottomLight != null)
            {
                getRender(bottomLight).SetFloat("_Alpha", alpha);
            }

            //修改描白边的透明度
            if (bottom != null)
            {
                getRender(bottom).SetFloat("_EdgeStrength", alpha);
            }
        }

        //起始不展示，后续一直展示，直到Clicked
        private void ChangeTopAlpha(float alpha)
        {
            if (top != null)
            {
                getRender(top).SetFloat("_Alpha", alpha);
            }
        }

        private System.Collections.IEnumerator AnimateColor(float duration)
        {
            float elapsedTime = 0f;
            var totalTime = duration;
            while (elapsedTime < totalTime)
            {
                float alpha = 1 - elapsedTime / totalTime;
                ChangeButtonAlpha(alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private void ShowTopOutline(bool isShow)
        {
            if (top != null)
            {
                var topRenderer = top.GetComponent<Renderer>();
                var material = topRenderer.material;
                material.SetFloat("_EdgeStrength", isShow ? 1 : 0);
            }
        }

        private TweenerCore<float, float, FloatOptions> fadeInTween;
        private TweenerCore<float, float, FloatOptions> fadeOutTween;

        public void FadeIn(float duration)
        {
            // 如果已经在执行渐入，先停止之前的渐入
            if (fadeOutTween != null && fadeOutTween.IsActive())
            {
                fadeOutTween.Kill();
            }

            var startAlpha = GetSelfAlpha();
            var endAlpha = enable ? 1f : 0.5f;

            fadeInTween = DOTween.To(() => startAlpha, SetSelfAlpha, endAlpha, duration);
        }

        public void FadeOut(float duration)
        {
            // 如果已经在执行渐入，先停止之前的渐入
            if (fadeInTween != null && fadeInTween.IsActive())
            {
                fadeInTween.Kill();
            }

            var startAlpha = GetSelfAlpha();
            var endAlpha = 0f;
            fadeOutTween = DOTween.To(() => startAlpha, SetSelfAlpha, endAlpha, duration);
        }

        public void SetSelfAlpha(float alpha)
        {
            if (top != null)
            {
                getRender(top).SetFloat("_Alpha", alpha);
            }

            if (middle != null)
            {
                getRender(middle).SetFloat("_Alpha", alpha);
            }

            if (bottom != null)
            {
                getRender(bottom).SetFloat("_Alpha", alpha);
            }

            if (bottomCollider != null)
            {
                getRender(bottomCollider).SetFloat("_Alpha", alpha);
            }
        }

        /**
         * 获取自身的 alpha，middle 层的透明度是永远不会变的
         */
        private float GetSelfAlpha()
        {
            return middle != null ? getRender(middle).GetFloat("_Alpha") : 0f;
        }

        private Material getRender(Transform obj)
        {
            var render = obj.GetComponent<Renderer>();
            return render.material;
        }

        private bool enable = true;

        public bool Enable
        {
            get => enable;
            set
            {
                SetEnable(value);
                enable = value;
            }
        }

        private void SetEnable(bool enable)
        {
            SetSelfAlpha(enable ? 1f : 0.5f);
        }

        /**
         * 手指戳到前面去了，导致射线没有检测到任何的物体
         * 
         */
        public void onLossRay()
        {
            if (!isClicking)
            {
                onLoss(5);
            }
        }

        private void PlayAudio(AudioClip audioClip)
        {
            if (audioClip != null)
            {
                if (audioClip == ElectronicClip)
                {
                    audioSource.volume = 0.2f;
                }
                else
                {
                    audioSource.volume = 1;
                }

                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}