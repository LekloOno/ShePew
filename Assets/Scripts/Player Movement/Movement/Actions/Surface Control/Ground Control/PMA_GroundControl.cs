using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_GroundControl : PMA_SurfaceControl<DATA_GroundControl>
{
    [Header("Specifics")]
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
        rb.drag = data.Drag;
        appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
        rb.AddForce(MovementPhysics.Acceleration(data.MaxSpeed, data.MaxAccel, rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
    }
}
