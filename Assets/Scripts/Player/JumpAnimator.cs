using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimator : MonoBehaviour
{
    private Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StarterAssets.FirstPersonController.onJump += PlayJump;
    }

    private void OnDisable()
    {
        StarterAssets.FirstPersonController.onJump -= PlayJump;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayJump()
    {
        _anim.Play("Hop", 0,0);
    }
}
