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

    public event EventHandler JumpQueue;

    Vector3 appliedDir;

    public DATA_SurfaceControl CurrentData;

    void FixedUpdate(){
        JumpQueue?.Invoke(this, EventArgs.Empty);
        /*if(_jump.JumpFrame){
            _jump.JumpFrame = false;
        } else {*/
            _rb.drag = CurrentData.Drag;
            appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
            _rb.AddForce(MovementPhysics.Acceleration(CurrentData.MaxSpeed, CurrentData.MaxAccel, _rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
        //}
    }
}
