using System;
using UnityEngine;
using DG.Tweening;
using XY.UXR;

namespace XY.UXR
{
    public class TimeMachineHandle : MonoBehaviour
    {
        private bool resetting = false;
        private void OnEnable()
        {
            GestureBarHelper.Instance.init(this);
            GestureBarHelper.Instance.onRotate += onRotate;
            GestureBarHelper.Instance.onLoss += onLoss;
        }

        private void OnDisable()
        {
            GestureBarHelper.Instance.onRotate -= onRotate;
            GestureBarHelper.Instance.onLoss -= onLoss;
            GestureBarHelper.Instance.deinit();
        }

        private void onRotate(float x)
        {
            if (!resetting)
            {
                float angle = Math.Clamp(x, -35f, 35f);
                if (Math.Abs(angle) > 34f)
                {
                    HandleReset();
                    int direction = angle > 0 ? 1 : -1;
                    // Do Something...
                }
                transform.localRotation = Quaternion.Euler(x, 0, 0);
            }
        }
        void HandleReset()
        {
            resetting = true;
            transform.DOLocalRotate(Vector3.zero, 1f).OnComplete(() =>
            {
                resetting = false;
            });
        }
        private void onLoss()
        {
            HandleReset();
        }
    }
}