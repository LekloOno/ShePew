using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Slide : PMA_Ability<DATA_Slide>
{
    [Header("Specifics")]
    public bool IsActive;

    public event EventHandler OnSlideStarted;
    public event EventHandler OnSlideStoped;

    [SerializeField] Transform player;
    [SerializeField] Transform playerDir;
    [SerializeField] PMA_GroundControl groundControl;
    [SerializeField] PMA_AirControl airControl;
    [SerializeField] PMA_GroundControl crouchControl;
    [SerializeField] PMA_AirControl airSlideControl;
    [SerializeField] PMA_SuperJump _superjumpHandler;

    [Header("Specifics/Behavior")]
    //[SerializeField] float crouchMaxSpeed;
    [SerializeField] float yScaleDownSpeed;
    [SerializeField] float yScaleUpSpeed;
    [SerializeField] float slideYScale;

    float startTime = 0;

    float currentScale = 1;
    float scaleSpeed;

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
        float realForce = Mathf.Pow(Mathf.Min(data.SlideDecayRecover, Time.time-startTime)/data.SlideDecayRecover,data.SlideDecayStrength);
        startTime = Time.time;
        InitiateSlide();
        rb.AddForce(_runningInput.WishDir * data.SlideXForce * realForce * (_groundState.IsGrounded ? 1 : data.AirForceMultiplier), ForceMode.Impulse);
        OnSlideStarted?.Invoke(this, EventArgs.Empty);
    }

    public void InitiateSlide()
    {
        Activate(true);
        currentScale = slideYScale;
        rb.drag = data.Drag;
        OnFixedUpdate += UpdateCrouch;
    }

    public override void StopAbility(InputAction.CallbackContext obj)
    {
        if(data.SlideDecayRecover-Time.time+startTime < data.SlideDecayMinRecover)
        {
            startTime = Time.time-(data.SlideDecayRecover-data.SlideDecayMinRecover);
        }
        _superjumpHandler.ResetCharge(false);
        StopSlide();
    }

    public void StopSlide()
    {
        Activate(false);
        crouchControl.enabled = false;
        currentScale = 1;
        scaleSpeed = yScaleUpSpeed;
        OnSlideStoped?.Invoke(this, EventArgs.Empty);
    }

    void Activate(bool state)
    {
        IsActive = state;
        groundControl.enabled = !state;
        airControl.enabled = !state;
        _superjumpHandler.enabled = state;
        airSlideControl.enabled = state;
    }

    void Scaling()
    {
        player.localScale = new Vector3(player.localScale.x, Mathf.Lerp(player.localScale.y, currentScale, scaleSpeed), player.localScale.z);
    }

    public void UpdateCrouch(object sender, EventArgs e)
    {
        if(IsActive)
        {
            if(_groundState.FlatVelocity <= groundControl.Data.MaxSpeed*0.75f && _groundState.IsGrounded)
            {
                crouchControl.enabled = true;
                OnFixedUpdate -= UpdateCrouch;
            }
        }
        else
        {
            crouchControl.enabled = false;
            OnFixedUpdate -= UpdateCrouch;
        }
    }
}
