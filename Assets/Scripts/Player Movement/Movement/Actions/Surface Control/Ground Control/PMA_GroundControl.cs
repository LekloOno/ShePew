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

    bool frameFreeDrag;
    
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
    }

    public override void ActivateData(){
        if(dataContainer != null){
            dataContainer.Data = currentData;
        }
    }

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        frameFreeDrag = false;
        ActivateData();
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
            rb.drag = data.Drag;
            appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
            rb.AddForce(MovementPhysics.Acceleration(currentData.MaxSpeed, currentData.MaxAccel, rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
        } else {
            frameFreeDrag = true;
        }
    }

    public void OnSprintDown(InputAction.CallbackContext obj){
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

    private void StartSprinting(){
        rb.drag = sprintData.Drag;
        currentData = sprintData;
        headBobber.enabled = true;
        ActivateData();
        isSprinting = true;
    }

    public void StopSprinting(){
        if(_groundState.IsGrounded)
            rb.drag = data.Drag;
        currentData = data;
        headBobber.enabled = false;
        ActivateData();
        isSprinting = false;
    }

    private void DirectKeyStopSprinting(){
        rb.drag = data.Drag;
        currentData = data;
        headBobber.enabled = false;
        ActivateData();
        isSprinting = false;
    }

}
