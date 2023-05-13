using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_SC_Manager : MonoBehaviour
{
    [SerializeField] private PIA_RunningProcessing _runningInput;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private Rigidbody _rb;
    
    public DATA_SurfaceControl CurrentData;

    void FixedUpdate(){
        appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
        _rb.AddForce(MovementPhysics.Acceleration(CurrentData.MaxSpeed, CurrentData.MaxAccel, _rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
    }
}
