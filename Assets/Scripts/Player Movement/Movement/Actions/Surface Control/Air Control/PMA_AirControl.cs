using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_AirControl : PMA_SurfaceControl<DATA_AirControl>
{
    void Start()
    {
        if(!_groundState.IsGrounded)
        {
            OnFixedUpdate -= OnAirBorn;
            OnFixedUpdate += OnAirBorn;
        }
    }   

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        OnFixedUpdate -= OnAirBorn;
    }

    public override void SurfaceControl_OnLeavingSurface(object sender, EventArgs e)
    {
        ActivateData();
        rb.drag = data.Drag;
        OnFixedUpdate += OnAirBorn;
    }

    void OnAirBorn(object sender, EventArgs e)
    {
        rb.AddForce(MovementPhysics.Acceleration(data.MaxSpeed, data.MaxAccel, rb.velocity, _runningInput.WishDir, _runningInput.WishDir), ForceMode.VelocityChange);
    }
}