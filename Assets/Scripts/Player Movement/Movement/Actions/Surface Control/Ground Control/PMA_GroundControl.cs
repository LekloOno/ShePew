using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_GroundControl : PMA_SurfaceControl<DATA_GroundControl>
{
    [Header("Specifics")]
    [SerializeField] PSS_Capsule pss_Capsule;
    Vector3 appliedDir;

    void Start()
    {
        if(_groundState.IsGrounded)
        {
            OnFixedUpdate -= OnGrounded;
            OnFixedUpdate += OnGrounded;
        }
    }

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        OnFixedUpdate += OnGrounded;
    }

    public override void SurfaceControl_OnLeavingSurface(object sender, EventArgs e)
    {
        OnFixedUpdate -= OnGrounded;
    }

    void OnGrounded(object sender, EventArgs e)
    {
        pss_Capsule.CurrentBaseMat = data.Mat; 
        rb.drag = data.Drag;
        appliedDir = Vector3.ProjectOnPlane(inputHandler.WishDir, _groundState.GroundNormal).normalized;
        rb.AddForce(MovementPhysics.Acceleration(data.MaxSpeed, data.MaxAccel, rb.velocity, inputHandler.WishDir, appliedDir), ForceMode.VelocityChange);
    }
}
