using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeakAni : MonoBehaviour
{
    public int AniNums;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAni()
    {
        int range = Random.Range(1, AniNums+1);
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.SetTrigger($"Speak{range}");
    }
}
