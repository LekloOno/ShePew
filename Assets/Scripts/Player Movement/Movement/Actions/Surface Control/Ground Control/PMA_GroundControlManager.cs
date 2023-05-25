using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_GroundControlManager : PMA_SurfaceControl<DATA_GroundControl>
{
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool enableSprint;
    [SerializeField] private bool _sprintAvailable = true;
    [SerializeField] private DATA_GroundControl sprintData;
    [SerializeField] private DATA_GroundControl baseData;
    [SerializeField] private PI_AMapsManager inputMapsManager;
    [SerializeField] private PC_Bobbing headBobber;
    [SerializeField] private PMA_Slide _slide;

    [SerializeField] private float _walkingSpeed = 5f;
    [SerializeField] private bool _isWalking = false;
    
    public EventHandler StopSprint;
    public EventHandler StartSprint;
    public EventHandler StartWalking;
    public EventHandler StopWalking;

    public EventHandler StartAirSprint;
    public EventHandler StopAirSprint;

    public bool EnableSprint{get => enableSprint;}


    bool frameFreeDrag = false;
    public bool IsSprinting {
        get => isSprinting;
    }

    protected override void Awake()
    {
        base.Awake();
        OnFixedUpdate += CheckWalking;
    }

    public void CheckWalking(object sender, EventArgs e)
    {
        bool currentlyWalking = _groundState.IsGrounded && !isSprinting && _groundState.FlatSpeed >= _walkingSpeed && !_slide.IsActive;
        if(currentlyWalking != _isWalking)
        {
            _isWalking = currentlyWalking;
            if(_isWalking)
            {
                StartWalking?.Invoke(this, EventArgs.Empty);
            }
            else if(!isSprinting)
            {
                StopWalking?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void AllowSprint(bool val)
    {
        _sprintAvailable = val;
    }

    public void ForceStopAirSprint()
    {
        StopAirSprint?.Invoke(this, EventArgs.Empty);
    }

    public void SetData(DATA_GroundControl newData)
    {
        data = newData;
        if(_groundState.IsGrounded)
            ActivateData();
    }

    public void ResetData()
    {
        SetData(baseData);
    }

    bool CanSprint()
    {
        return _sprintAvailable && enableSprint;
    }

    void Start()
    {
        data = baseData;
        if(_groundState.IsGrounded)
        {
            OnFixedUpdate -= OnGrounded;
            OnFixedUpdate += OnGrounded;
        }

        if(enableSprint)
        {
            inputMapsManager.playerInputActions.Arena.Sprint.performed += OnSprintDown;
            _runningInput.OnStopOrLess += Sprint_OnStopOrLess;
        }    
    }

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        frameFreeDrag = false;
        OnFixedUpdate += OnGrounded;
        StopAirSprint?.Invoke(this, EventArgs.Empty);
    }

    public override void SurfaceControl_OnLeavingSurface(object sender, EventArgs e)
    {
        /*if(enableSprint)
            StopSprinting();*/
        OnFixedUpdate -= OnGrounded;
        if(isSprinting) StartAirSprint?.Invoke(this, EventArgs.Empty);
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
        if(_groundState.IsGrounded && CanSprint())
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
        //headBobber.Enable = true;
        if(!_runningInput.StopOrLess())
        {
            StartSprint?.Invoke(this, EventArgs.Empty);
            data = sprintData;
            ActivateData();
            isSprinting = true;
        }
    }

    public void StopSprinting(){
        if(isSprinting){
            data = baseData;
            if(_groundState.IsGrounded)
                ActivateData();
            //headBobber.Enable = false;
            StopSprint?.Invoke(this, EventArgs.Empty);
            isSprinting = false;
        }
    }

    private void DirectKeyStopSprinting(){
        data = baseData;
        ActivateData();
        //headBobber.Enable = false;
        StopSprint?.Invoke(this, EventArgs.Empty);
        isSprinting = false;
    }
}
