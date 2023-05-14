using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_DriftControl : PMA_SurfaceControl<DATA_DriftControl>
{
    [Header("Specifics")]
    [SerializeField] PMA_Slide slideHandler;
    [SerializeField] PMA_SuperJump _superjumpHandler;
    [SerializeField] Transform playerDir;
    Vector3 appliedDir;

    float cacheHeight;
    float cacheVelocity;
    float slideTime;
    public float SlideTime{get => slideTime;}
    float duration;
    public float Duration{get => duration;}
    protected override void Awake()
    {
        base.Awake();
        slideHandler.OnSlideStarted += Drift_OnSlideStarted;
        slideHandler.OnSlideStoped += Drift_OnSlideStoped;
    }

    public override void SurfaceControl_OnLandingSurface(object sender, EventArgs e)
    {
        OnFixedUpdate += OnGrounded;
    }

    public override void SurfaceControl_OnLeavingSurface(object sender, EventArgs e)
    {
        OnFixedUpdate -= OnGrounded;
        if(this.enabled)
        {
            this.enabled = false;
            //slideHandler.InitiateSlide();
            _superjumpHandler.enabled = true;
        }
    }

    void OnGrounded(object sender, EventArgs e)
    {
        if(slideTime >= duration)
        {
            this.enabled = false;
            //slideHandler.InitiateSlide();
            _superjumpHandler.enabled = true;
        }
        else
            slideTime += Time.fixedDeltaTime;
        rb.drag = data.Drag;
        appliedDir = Vector3.ProjectOnPlane(_runningInput.WishDir, _groundState.GroundNormal).normalized;
        Vector3 perp = Vector3.Cross(Vector3.up, playerDir.forward).normalized;
        Debug.DrawRay(transform.position, -Vector3.Dot(rb.velocity, perp)*perp);
        rb.AddForce(-Vector3.Dot(rb.velocity, perp)*perp*Time.fixedDeltaTime*data.TractionDrag + MovementPhysics.Acceleration(data.MaxSpeed, data.MaxAccel, rb.velocity, _runningInput.WishDir, appliedDir), ForceMode.VelocityChange);
    }

    void Drift_OnSlideStoped(object sender, EventArgs e)
    {
        this.enabled = false;
    }

    void Drift_OnSlideStarted(object sender, EventArgs e)
    {
        //cacheHeight = _groundState.GroundHeight;
        cacheVelocity = -rb.velocity.y;
        if(/*cacheHeight < data.MaxHeight && */!_groundState.IsGrounded && cacheVelocity > 0 && Time.time-_runningInput.LastBackward < data.BackBuffer)
        {
            float maxVel = Mathf.Sqrt(2*-Physics.gravity.y*(data.MaxHeight-1));
            float x = cacheVelocity/maxVel;
            float f = Mathf.Min(Mathf.Pow(x,2)*(data.MaxDuration-data.MinDuration)+data.MinDuration, data.MaxDuration);
            float g = Mathf.Max(0, -1/(x/(data.RealMaxDuration-data.MaxDuration))+data.RealMaxDuration-data.MaxDuration);
            slideHandler.OnFixedUpdate -= slideHandler.UpdateCrouch;
            /*
            float k = (cacheHeight-1)/(data.MaxHeight-1);
            duration = Mathf.Pow(-k+1,data.DurationStrength)*(data.MaxDuration-data.MinDuration)+data.MinDuration;*/
            duration = f+g;
            Debug.Log(duration);
            slideTime = 0;
            //https://www.desmos.com/calculator/0szp3lis4u
            this.enabled = true;
            _superjumpHandler.enabled = false;
        }
    }
}
