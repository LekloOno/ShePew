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
    [SerializeField] private float _wideLurchPenalty = 0.7f;
    //The velocity coefficient of a Direct Lurch, 1 means no penalty.
    [SerializeField] private float _momentumLurchPenalty = 0.05f;
    [SerializeField] private float _maximumMomentumLurchKMPenalty = 5f; //In km/h
    private float _maximumMomentumLurchPenalty;
    //The velocity coefficent of a Momentum Lurch, 1 means no penalty.

    private static string _airDriftModifierKey = "AirDrift";
    [SerializeField] private float _airDriftSpeedModifier;
    [SerializeField] private float _airDriftAccelModifier;

    private float _maxSteps;    //Max Time converted in engine ticks.
    private float _maxDot;      //Max Lurch Angle converted into the max Dot Product.
    private float _maxFullLurch;//Max Full Lurch Angle converted into the max Dot Product.


    void Start()
    {
        _maximumMomentumLurchPenalty = _maximumMomentumLurchKMPenalty/3.6f;
        _maxDot = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _maxAngle));
        _maxFullLurch = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _maxFullLurchAngle));
        Debug.Log(_maxDot);
        _maxSteps = _maxTime/Time.fixedDeltaTime;
        _runningInput.KeyPressed += Lurch_OnKeyPressed;
        _groundState.OnLanding += OnLanding_AirDriftEnd;
    }

    public void Lurch_OnKeyPressed(object sender, KeyPressedArgs args)
    {
        float absDot = Mathf.Abs(Vector3.Dot(_groundState.FlatVelocity.normalized, args.WishDir));
        if(!_groundState.IsGrounded && _jumpProcessing.StepSinceLastJumped < _maxSteps && absDot<=_maxDot && args.WishDir != Vector3.zero)
        {
            if(args.RunningAxis.y > 0)
            {
                //AIR DRIFT
                _surfaceControlManager.ActivateAirDrift();
                _surfaceControlManager.MaxSpeedModifiers[_airDriftModifierKey] = _airDriftSpeedModifier;
                _surfaceControlManager.MaxAccelModifiers[_airDriftModifierKey] = _airDriftAccelModifier;
                Invoke("AirDriftEnd", (_maxSteps-_jumpProcessing.StepSinceLastJumped)*Time.fixedDeltaTime);
            }
            else
            {
                if(absDot < _maxFullLurch)
                {
                    //MOMENTUM LURCH
                    Vector3 lerpedDir = Vector3.Lerp(_groundState.FlatVelocity.normalized, args.WishDir, 0.5f).normalized;
                    float speedPenalty = Mathf.Min(_groundState.FlatSpeed * _momentumLurchPenalty, _maximumMomentumLurchPenalty);
                    _rb.velocity = lerpedDir*(_groundState.FlatSpeed - speedPenalty) + new Vector3(0, _rb.velocity.y, 0);
                }
                else
                {
                    //DIRECT LURCH
                    _rb.velocity = args.WishDir.normalized * _groundState.FlatSpeed * _wideLurchPenalty + new Vector3(0, _rb.velocity.y, 0);
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
