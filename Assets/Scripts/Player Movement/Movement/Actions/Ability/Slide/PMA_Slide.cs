using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Slide : PMA_Ability<DATA_Slide>
{
    [Header("Specifics")]
    public bool IsActive;

    public event EventHandler OnInputIn;
    public event EventHandler OnScaleDownComplete;
    public event EventHandler OnSlideStarted;
    public event EventHandler OnSlideStoped;

    [SerializeField] Transform _player;
    [SerializeField] PMA_GroundControlManager _groundControlManager;
    //[SerializeField] PMA_AirControl airControl;
    [SerializeField] DATA_GroundControl _crouchControl;
    //[SerializeField] DATA_AirControl _airSlideControl;
    [SerializeField] DATA_GroundControl _slideGroundControl;

    [Header("Specifics/Behavior")]
    //[SerializeField] float crouchMaxSpeed;
    [SerializeField] float yScaleDownSpeed;
    [SerializeField] float yScaleUpSpeed;
    [SerializeField] float slideYScale;
    [SerializeField] float slideMinSpeed = 7.8f;

    float startTime = 0;

    float currentScale = 1;
    float scaleSpeed;
    private float _realForce;

    protected override void Start()
    {
        action = inputMapsManager.playerInputActions.Arena.Slide;
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Scaling();
    }

    public override void StartAbility(InputAction.CallbackContext obj)
    {
        OnInputIn?.Invoke(this, EventArgs.Empty);
        currentScale = slideYScale;
        scaleSpeed = yScaleDownSpeed;
        _groundControlManager.StopSprinting();
        _groundControlManager.AllowSprint(false);
        
        if(_groundState.FlatVelocity > slideMinSpeed)
        {
            _realForce = Mathf.Pow(Mathf.Min(data.SlideDecayRecover, Time.time-startTime)/data.SlideDecayRecover,data.SlideDecayStrength);
            startTime = Time.time;
            _groundControlManager.SetData(_slideGroundControl);
            IsActive = true;
            OnFixedUpdate += UpdateCrouch;
            OnSlideStarted?.Invoke(this, EventArgs.Empty);
            Invoke("ApplySlideForce", data.SlideForceDelay);
        }
        else
        {
            _groundControlManager.SetData(_crouchControl);
        }
    }

    void ApplySlideForce()
    {
        rb.AddForce(_runningInput.WishDir * data.SlideXForce * _realForce * (_groundState.IsGrounded ? 1 : data.AirForceMultiplier), ForceMode.Impulse);
    }

/*
    public void InitiateSlide()
    {
        Activate(true);
        currentScale = slideYScale;
        rb.drag = data.Drag;
        OnFixedUpdate += UpdateCrouch;
    }
*/
    public override void StopAbility(InputAction.CallbackContext obj)
    {
        if(data.SlideDecayRecover-Time.time+startTime < data.SlideDecayMinRecover)
        {
            startTime = Time.time-(data.SlideDecayRecover-data.SlideDecayMinRecover);
        }
        StopSlide();
    }

    public void StopSlide()
    {
        //Activate(false);
        CancelInvoke("ApplySlideForce");
        IsActive = false;
        _groundControlManager.AllowSprint(true);
        _groundControlManager.ResetData();
        currentScale = 1;
        scaleSpeed = yScaleUpSpeed;
        OnSlideStoped?.Invoke(this, EventArgs.Empty);
    }

/*
    void Activate(bool state)
    {
        IsActive = state;
        _groundControlManager.enabled = !state;
        airControl.enabled = !state;
        _airSlideControl.enabled = state;
    }*/

    void Scaling()
    {
        _player.localScale = new Vector3(_player.localScale.x, Mathf.Lerp(_player.localScale.y, currentScale, scaleSpeed), _player.localScale.z);
        if(Mathf.Abs(_player.localScale.y - currentScale) < 0.05f){
            OnScaleDownComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public void UpdateCrouch(object sender, EventArgs e)
    {
        if(IsActive)
        {
            if(_groundState.FlatVelocity <= _crouchControl.MaxSpeed*0.8f && _groundState.IsGrounded)
            {
                _groundControlManager.SetData(_crouchControl);
                OnFixedUpdate -= UpdateCrouch;
            }
        }
        else
        {
            _groundControlManager.ResetData();
            OnFixedUpdate -= UpdateCrouch;
        }
    }
}
