using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Lurch : MonoBehaviour
{
    [SerializeField] private PIA_RunningProcessing _runningInput;
    [SerializeField] private PI_AMapsManager inputsMM;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _maxTime;    //Max Time after a jump in Seconds.
    [SerializeField] private float _maxAngle;   //Max Lurch Angle in Degrees.

    //Direct Lurch and Full Lurch
    [SerializeField] private float _maxFullLurchAngle = 180;
    //Max Angle for a classic Full Lurch. Beyond this angle, the redirection will be completely based on wishDir, but with a loss of speed, a Direct Lurch.
    [SerializeField] private float _wideLurchPenalty = 1;
    //The coefficient of a Direct Lurch velocity, 1 means no penalty.

    private float _maxSteps;    //Max Time converted in engine ticks.
    private float _maxDot;      //Max Lurch Angle converted into the max Dot Product.
    private float _maxFullLurch;//Max Full Lurch Angle converted into the max Dot Product.


    void Start()
    {
        _maxDot = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _maxAngle));
        _maxFullLurch = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _maxFullLurchAngle));
        Debug.Log(_maxDot);
        _maxSteps = _maxTime/Time.fixedDeltaTime;
        inputsMM.playerInputActions.Arena.Running.performed += Lurch_OnKeyPressed2;
    }

    public void Lurch_OnKeyPressed2(InputAction.CallbackContext obj)
    {
        Vector3 localWishDir = _runningInput.FreeWishDir(obj.ReadValue<Vector2>());
        float absDot = Mathf.Abs(Vector3.Dot(_groundState.FlatVelocity.normalized, localWishDir));
        if(!_groundState.IsGrounded && _groundState.StepSinceLastJumped < _maxSteps && absDot<_maxDot)
        {
            if(absDot < _maxFullLurch)
            {
                Debug.Log("FULL LURCH");
                Vector3 lerpedDir = Vector3.Lerp(_groundState.FlatVelocity.normalized, localWishDir, 0.5f).normalized;
                _rb.velocity = lerpedDir*_groundState.FlatSpeed + new Vector3(0, _rb.velocity.y, 0);
            }
            else
            {
                Debug.Log("DEBUFF");
                _rb.velocity = localWishDir.normalized * _groundState.FlatSpeed * _wideLurchPenalty + new Vector3(0, _rb.velocity.y, 0);
            }
        }
    }
}
