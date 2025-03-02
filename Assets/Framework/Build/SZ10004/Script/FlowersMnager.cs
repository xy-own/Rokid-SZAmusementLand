using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture.Button;

namespace SZ10004
{
    public class FlowersMnager : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponent<BtnItem>().enterAction += ClickFlower;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ClickFlower(FingerEvent finger, Collider go)
        {
            //if (gameObject.GetComponent<Animator>().enabled)
            //{
            //    gameObject.GetComponent<Animator>().enabled = false;
            //}
            gameObject.GetComponent<Animator>().SetTrigger("Yao");
        }
    }
}

