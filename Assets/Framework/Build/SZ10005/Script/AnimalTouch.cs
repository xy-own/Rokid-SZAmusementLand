using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XY.UXR.Gesture.Button;

public class AnimalTouch : MonoBehaviour
{
    public int AniIndex;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BtnItem>().enterAction += ClickAnimal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickAnimal(FingerEvent finger, Collider go)
    {
        int range = Random.Range(1, AniIndex + 1);
        gameObject.GetComponent<Animator>().SetTrigger($"Touch{range}");
    }
}
