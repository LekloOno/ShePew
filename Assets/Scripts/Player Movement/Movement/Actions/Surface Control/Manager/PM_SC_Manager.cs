using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(51)]
public class PM_SC_Manager : MonoBehaviour
{
    [SerializeField] private PIA_RunningProcessing _runningInput;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PMA_Jump _jump;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _groundStopSpeed = 1f;
    private bool _isAirDrifting = false;

    public Dictionary<string, float> MaxSpeedModifiers = new Dictionary<string, float>();
    public Dictionary<string, float> MaxAccelModifiers = new Dictionary<string, float>();
    public Dictionary<string, float> DragModifiers = new Dictionary<string, float>();
    
    public event EventHandler JumpQueue;

    Vector3 appliedDir;

    public DATA_SurfaceControl CurrentData;
    private DATA_SurfaceControl _prevData;

    void FixedUpdate(){
        JumpQueue?.Invoke(this, EventArgs.Empty);
        /*if(_jump.JumpFrame){
            _jump.JumpFrame = false;
        } else {*/
            _rb.drag = Drag();
            appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
            if(_isAirDrifting && !_groundState.IsGrounded)
            {
                Vector3 acceleratedVel = _rb.velocity + MovementPhysics.Acceleration(MaxSpeed(), MaxAccel(), _rb.velocity, _runningInput.WishDir, appliedDir);
                _rb.velocity = acceleratedVel.normalized * Mathf.Min(acceleratedVel.magnitude, _groundState.FlatSpeed);
            }
            else
            {
                _rb.AddForce(MovementPhysics.Acceleration(MaxSpeed(), MaxAccel(), _rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
                if(_groundState.IsGrounded && _groundState.FlatSpeed < _groundStopSpeed) _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            }
        //}
    }

    public void ActivateAirDrift()
    {
        //_prevData = CurrentData;
        _isAirDrifting = true;
    }

    public void DeactivateAirDrift()
    {
        //CurrentData = _prevData;
        _isAirDrifting = false;
    }

    public float Drag()
    {
        return CurrentData.Drag * Modifier(DragModifiers.Values);
    }

    public float MaxSpeed()
    {
        return CurrentData.MaxSpeed*Modifier(MaxSpeedModifiers.Values);
    }

    public float MaxAccel()
    {
        return CurrentData.MaxAccel*Modifier(MaxAccelModifiers.Values);
    }

    private float Modifier(Dictionary<string, float>.ValueCollection values)
    {
        float modifier = 1;
        foreach(float m in values)
        {
            modifier *= m;
        }
        return modifier;
    }

}
