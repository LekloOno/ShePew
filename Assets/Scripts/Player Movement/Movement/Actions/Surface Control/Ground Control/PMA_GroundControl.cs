using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_GroundControl : PMA_SurfaceControl<DATA_GroundControl>
{
    [Header("Specifics")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private DATA_GroundControl sprintData;
    [SerializeField] private PI_AMapsManager inputMapsManager;
    [SerializeField] private PC_Bobbing headBobber;

    bool frameFreeDrag = false;
    
    private DATA_GroundControl currentData;

    public bool IsSprinting {
        get => isSprinting;
    }
    Vector3 appliedDir;

    void Start()
    {
        currentData = data;
        if(_groundState.IsGrounded)
        {
            OnFixedUpdate -= OnGrounded;
            OnFixedUpdate += OnGrounded;
        }

        inputMapsManager.playerInputActions.Arena.Sprint.performed += OnSprintDown;
        _runningInput.OnStopOrLess += Sprint_OnStopOrLess;
    }

    public override void ActivateData(){
        surfaceControlManager.CurrentData = currentData;
    }

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        frameFreeDrag = false;
        OnFixedUpdate += OnGrounded;
        
    }

    public override void SurfaceControl_OnLeavingSurface(object sender, EventArgs e)
    {
        StopSprinting();
        OnFixedUpdate -= OnGrounded;
    }

    void OnGrounded(object sender, EventArgs e)
    {
        if(frameFreeDrag){
            ActivateData();
            OnFixedUpdate -= OnGrounded;
        } else {
            frameFreeDrag = true;
        }
    }

    public void OnSprintDown(InputAction.CallbackContext obj)
    {
        if(_groundState.IsGrounded)
        {
            if(isSprinting)
            {
                DirectKeyStopSprinting();
            }
            else
            { 
                StartSprinting();
            }
        }   
    }

    public void Sprint_OnStopOrLess(object sender, EventArgs e)
    {
        StopSprinting();
    }

    private void StartSprinting(){
        headBobber.Enable = true;
        currentData = sprintData;
        ActivateData();
        isSprinting = true;
    }

    public void StopSprinting(){
        currentData = data;
        if(_groundState.IsGrounded)
            ActivateData();
        headBobber.Enable = false;
        isSprinting = false;
    }

    private void DirectKeyStopSprinting(){
        currentData = data;
        ActivateData();
        headBobber.Enable = false;
        isSprinting = false;
    }

}
