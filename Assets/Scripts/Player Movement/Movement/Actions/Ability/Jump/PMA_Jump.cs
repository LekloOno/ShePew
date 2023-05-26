using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[DefaultExecutionOrder(50)]
public class PMA_Jump : PMA_Ability<DATA_Jump>
{
    [SerializeField] private PIA_JumpProcessing _jumpProcessing;
    [SerializeField] private PMA_AirControl _airControl;
    [SerializeField] private PMA_GroundControlManager _groundControl;
    [SerializeField] private PM_SC_Manager _surfaceControlManager;

    [Header("Specifics/Behavior")]
    private static string SpeedModifierKey = "JumpHoldSpeedPenalty";
    [SerializeField] float resetMaxTime = 0.05f;                        //If the player jump without leaving the ground, the jump wont be reseted. This forces the jump to be reseted after this amount of time.
    [SerializeField] float speedPenaltyBuffer = 0.15f;                  //Time before the hold Speed Penalty is applied

    [Header("Debugging")]
    [SerializeField] float tracker_heldJumpCD = 0;
    [SerializeField] float tracker_heldJumpDelay;
    [SerializeField] float tracker_heldJumpThreshold;
    [SerializeField] float tracker_jumpDecayRecover;

    [SerializeField] bool canJump = true;

    public EventHandler<JumpEventArgs> OnJumped;

    #region Setup

    protected override void Start()
    {
        action = inputMapsManager.playerInputActions.Arena.Jump;
        base.Start();
        
        _groundState.OnLeavingGround += Jump_OnLeavingGround;
        _groundState.OnLanding += Jump_OnLanding;
    }

    #endregion

    public override void StartAbility(InputAction.CallbackContext obj)
    {
        Invoke("ApplySpeedPenalty", speedPenaltyBuffer);
        if(canJump)
        {
            tracker_heldJumpThreshold = data.HeldJumpThreshold;

            OnFixedUpdate += OnJump;
        }
    }

    public override void StopAbility(InputAction.CallbackContext obj)
    {
        CancelInvoke("ApplySpeedPenalty");
        _surfaceControlManager.MaxSpeedModifiers.Remove(SpeedModifierKey);
        OnFixedUpdate -= OnJump;
        tracker_heldJumpCD = 0;
    }

    void ApplySpeedPenalty()
    {
        _surfaceControlManager.MaxSpeedModifiers[SpeedModifierKey] = Data.HoldSpeedPenalty;
    }

    void OnJump(object sender, EventArgs e)
    {
        if(canJump && _groundState.IsGrounded)
        {
            if(tracker_heldJumpThreshold > 0)
            {
                Jump(data.TapJumpForce);
            }
            else
            {
                if(tracker_heldJumpCD > 0)
                    tracker_heldJumpCD -= Time.fixedDeltaTime;
                else
                {
                    if(tracker_heldJumpDelay > 0)
                        tracker_heldJumpDelay -= Time.fixedDeltaTime;
                    else
                        Jump(data.HeldJumpForce);
                }
            }
        }
        else
        {
            tracker_heldJumpCD -= Time.fixedDeltaTime;
            tracker_heldJumpThreshold -= Time.fixedDeltaTime;
        }
    }

    void Jump(float force)
    {
        canJump = false;
        Invoke("ForceReset", resetMaxTime);

        if(_groundControl.EnableSprint) _groundControl.StopSprinting();
        rb.drag = _airControl.Data.Drag;
        _surfaceControlManager.CurrentData = _airControl.Data;

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y*data.PreYMulltiplier, rb.velocity.z);

        float recoverMultiplier;
        float currentState = 1-(tracker_jumpDecayRecover/data.JumpDecayRecover);
        if(data.JumpDecayRecover > 0)
            recoverMultiplier = Mathf.Lerp(data.JumpDecayFloor, 1, data.JumpDecayCurve.Evaluate(currentState));
        else
            recoverMultiplier = 1;

        rb.AddForce(Vector3.up*force*recoverMultiplier, ForceMode.Impulse);

        tracker_heldJumpCD = data.HeldJumpCD;
        tracker_heldJumpDelay = data.HeldJumpDelay;

        float currentRecover = tracker_jumpDecayRecover;
        tracker_jumpDecayRecover = data.JumpDecayRecover;
        if(currentRecover <= 0)
            OnFixedUpdate += RecoverDecay;

        _jumpProcessing.ResetJumpTracker();
        OnJumped?.Invoke(this, new JumpEventArgs((force*recoverMultiplier)/data.TapJumpForce));
    }

    void RecoverDecay(object sender, EventArgs e)
    {
        if(tracker_jumpDecayRecover > 0)
            tracker_jumpDecayRecover -= Time.fixedDeltaTime;
        else
        {
            tracker_jumpDecayRecover = 0;
            OnFixedUpdate -= RecoverDecay;
        }
    }

    public void Jump_OnLeavingGround(object sender, EventArgs e)
    {
        canJump = true;
    }

    public void Jump_OnLanding(object sender, EventArgs e)
    {
        if(_jumpProcessing.UseBuffer())
        {
            Jump(data.TapJumpForce);
        }
    }

    void ForceReset()
    {
        canJump = true;
        if(_groundState.IsGrounded)
        {
            _surfaceControlManager.CurrentData = _groundControl.Data;
        }          
    }
}
