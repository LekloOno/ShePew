using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Lurch : MonoBehaviour
{
    [SerializeField] private PIA_RunningProcessing _runningInput;
    [SerializeField] private PIA_JumpProcessing _jumpProcessing;
    [SerializeField] private PI_AMapsManager inputsMM;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PM_SC_Manager _surfaceControlManager;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _maxTime;    //Max Time after a jump in Seconds.
    [SerializeField] private float _maxAngle = 180f;   //Max Lurch Angle in Degrees.

    [Header("Direct and Momentum Lurch")]
    [SerializeField] private float _maxFullLurchAngle = 160f;
    //Max Angle for a Momentum Lurch. Beyond this angle, the redirection will be completely based on wishDir, but with a loss of speed, a Direct Lurch.
    [SerializeField] private float _directLurchPenalty = 0.3f;
    //The velocity coefficient of a Direct Lurch, 0 means no penalty.
    [SerializeField] private float _momentumLurchPenalty = 0.05f;
    //The velocity coefficent of a Momentum Lurch, 0 means no penalty.
    [SerializeField] private float _maximumMomentumLurchKMPenalty = 5f; //In km/h
    [SerializeField] private float _maximumDirectLurchKMPenalty = 12f; //In km/h
    private float _maximumMomentumLurchPenalty;
    private float _maximumDirectLurchPenalty;

    private static string _airDriftModifierKey = "AirDrift";
    [SerializeField] private float _airDriftSpeedModifier;
    [SerializeField] private float _airDriftAccelModifier;

    private float _maxSteps;    //Max Time converted in engine ticks.
    private float _maxDot;      //Max Lurch Angle converted into the max Dot Product.
    private float _maxFullLurch;//Max Full Lurch Angle converted into the max Dot Product.


    void Start()
    {
        _maximumMomentumLurchPenalty = _maximumMomentumLurchKMPenalty/3.6f;
        _maximumDirectLurchPenalty = _maximumDirectLurchKMPenalty/3.6f;
        _maxDot = Mathf.Cos(Mathf.Deg2Rad * _maxAngle + Mathf.PI);
        _maxFullLurch = Mathf.Cos(Mathf.Deg2Rad * _maxFullLurchAngle + Mathf.PI);
        Debug.Log("Max dot : " + _maxDot + " | MaxFull dot : " + _maxFullLurch);
        _maxSteps = _maxTime/Time.fixedDeltaTime;
        _runningInput.KeyPressed += Lurch_OnKeyPressed;
        _groundState.OnLanding += OnLanding_AirDriftEnd;
    }

    public void Lurch_OnKeyPressed(object sender, KeyPressedArgs args)
    {
        float angleDot = Vector3.Dot(_groundState.FlatVelocity.normalized, args.WishDir.normalized);
        Debug.Log(angleDot);
        if(!_groundState.IsGrounded && _jumpProcessing.StepSinceLastJumped < _maxSteps && angleDot<=_maxDot && args.WishDir != Vector3.zero)
        {
            if(args.RunningAxis.y > 0 && args.RunningAxis.x != 0)
            {
                //AIR DRIFT
                _surfaceControlManager.ActivateAirDrift();
                _surfaceControlManager.MaxSpeedModifiers[_airDriftModifierKey] = _airDriftSpeedModifier;
                _surfaceControlManager.MaxAccelModifiers[_airDriftModifierKey] = _airDriftAccelModifier;
                Invoke("AirDriftEnd", (_maxSteps-_jumpProcessing.StepSinceLastJumped)*Time.fixedDeltaTime);
            }
            else
            {
                if(angleDot < _maxFullLurch)
                {
                    //MOMENTUM LURCH
                    Vector3 lerpedDir = Vector3.Lerp(_groundState.FlatVelocity.normalized, args.WishDir, 0.5f).normalized;
                    float speedPenalty = Mathf.Min(_groundState.FlatSpeed * _momentumLurchPenalty, _maximumMomentumLurchPenalty);
                    _rb.velocity = lerpedDir*(_groundState.FlatSpeed - speedPenalty) + new Vector3(0, _rb.velocity.y, 0);
                }
                else
                {
                    //DIRECT LURCH
                    float speedPenalty = Mathf.Min(_groundState.FlatSpeed * _directLurchPenalty, _maximumDirectLurchPenalty);
                    _rb.velocity = args.WishDir.normalized * (_groundState.FlatSpeed - speedPenalty) + new Vector3(0, _rb.velocity.y, 0);
                }
            }
        }
    }

    void AirDriftEnd()
    {
        _surfaceControlManager.MaxSpeedModifiers.Remove(_airDriftModifierKey);
        _surfaceControlManager.MaxAccelModifiers.Remove(_airDriftModifierKey);
        _surfaceControlManager.DeactivateAirDrift();
    }

    public void OnLanding_AirDriftEnd(object sender, EventArgs e)
    {
        AirDriftEnd();
    }
}
