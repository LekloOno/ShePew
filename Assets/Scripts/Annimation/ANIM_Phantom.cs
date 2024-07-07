using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ANIM_Phantom : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PMA_GroundControlManager _groundControl;
    [SerializeField] private PMA_Slide _slide;
    [SerializeField] private PI_AMapsManager _AMapsManager;

    void Start()
    {
        _groundControl.StartSprint += OnStartSprint;
        _groundControl.StopSprint += OnStopSprint;
        _groundControl.StartAirSprint += OnStartAirSprint;
        _groundControl.StopAirSprint += OnStopAirSprint;
        _slide.OnInputIn += OnSlideStarted;
        _slide.OnSlideStoped += OnSlideStoped;
        _AMapsManager.playerInputActions.Arena.WeaponSecondary.performed += OnStartAiming;
        _AMapsManager.playerInputActions.Arena.WeaponSecondary.canceled += OnStopAiming;
    }

    public void OnStartAiming(InputAction.CallbackContext obj)
    {
        Debug.Log("zizi");
        _animator.SetBool("IsAiming", true);
    }

    public void OnStopAiming(InputAction.CallbackContext obj)
    {
        Debug.Log("caca");
        _animator.SetBool("IsAiming", false);
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsSprinting", true);
    }

    public void OnStopSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsSprinting", false);
    }

    public void OnStartAirSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsAirSprinting", true);
    }

    public void OnStopAirSprint(object sender, EventArgs e)
    {
        _animator.SetBool("IsAirSprinting", false);
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
