using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ANIM_Phantom : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PMA_GroundControlManager _groundControl;
    [SerializeField] private PMA_Slide _slide;

    void Start()
    {
        _groundControl.StartSprint += OnStartSprint;
        _groundControl.StopSprint += OnStopSprint;
        _slide.OnSlideStarted += OnSlideStarted;
        _slide.OnSlideStoped += OnSlideStoped;
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsSprinting", true);
    }

    public void OnStopSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsSprinting", false);
    }

    public void OnSlideStarted(object sender, EventArgs e)
    {
        _animator.SetBool("IsSliding", true);
    }

    public void OnSlideStoped(object sender, EventArgs e)
    {
        _animator.SetBool("IsSliding", false);
    }
}
