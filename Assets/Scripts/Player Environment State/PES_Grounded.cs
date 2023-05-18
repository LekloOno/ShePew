using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PES_Grounded : MonoBehaviour
{
    [Header("States")]
    public bool IsGrounded;


    public float GroundHeight;
    public float GroundAngle;
    public Vector3 GroundNormal;

    public event EventHandler OnLanding;
    public event EventHandler<LandingEventArgs> OnLandingInfos;
    public event EventHandler OnLeavingGround;

    [Header("Rigidbody")]
    public float FlatSpeed;
    public Vector3 Velocity;
    public Vector3 FlatVelocity;
    
    [Header("Tweak")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform spatial;
    [SerializeField] Transform body;
    [SerializeField] LayerMask ground;
    [SerializeField] float distToGround;
    [SerializeField] float groundedRadius;
    [SerializeField] float groundedMaxAngle;
    [SerializeField] float maxVerticalSpeed;

    public LayerMask Ground { get=> ground;}

    RaycastHit groundHit;
    RaycastHit groundedHit;

    float groundInfoRadius;
    float realDistToGround;

    private float _stepSinceLastGrounded = 0;
    private float _stepSinceLastJumped = 5;
    private float _minJumpStep = 5;

    [SerializeField] bool _showLog;

    void Awake()
    {
        float R2 = body.localScale.x*0.5f;
        float R1_2 = Mathf.Pow(groundedRadius, 2);
        float R2_2 = Mathf.Pow(body.localScale.x*0.5f,2);

        float d = Vector3.Distance(body.position+Vector3.down*(spatial.localScale.y-R2), body.position+(distToGround-groundedRadius)*Vector3.down);

        float a = ((R1_2-R2_2)/(-d)-d)/2;

        groundInfoRadius = Mathf.Sqrt(R1_2 - Mathf.Pow(a,2));
    }

    void FixedUpdate()
    {
        _stepSinceLastGrounded ++;
        _stepSinceLastJumped ++;
        realDistToGround = transform.localScale.y*distToGround;
        Velocity = rb.velocity;
        FlatVelocity = (new Vector3(Velocity.x,0,Velocity.z));
        FlatSpeed = FlatVelocity.magnitude;

        if(Physics.Raycast(spatial.position, Vector3.down, out groundHit, Mathf.Infinity, ground))
        {
            GroundHeight = groundHit.distance;
        }

        if(Physics.SphereCast(spatial.position, groundedRadius, Vector3.down, out groundedHit, realDistToGround-groundedRadius, ground))
        {
            GroundNormal = groundedHit.normal;
            GroundAngle = Vector3.Angle(Vector3.up, groundedHit.normal);
            UpdateGrounded(GroundAngle<=groundedMaxAngle);
        }
        else
        {
            GroundNormal = Vector3.up;
            GroundAngle = 0;
            UpdateGrounded(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Debug.DrawLine(spatial.position, spatial.position + Vector3.down*(GroundHeight-groundedRadius));
        Gizmos.DrawWireSphere(spatial.position+Vector3.down*(GroundHeight-groundInfoRadius), groundInfoRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spatial.position+Vector3.down*Mathf.Min(GroundHeight-groundedRadius,realDistToGround-groundedRadius), groundedRadius);
    }

    public void UpdateGrounded(bool nextGrounded)
    {
        if(Mathf.Abs(rb.velocity.y) >= maxVerticalSpeed && _showLog) Debug.Log("Too fast");
        bool realNextGrounded = rb.velocity.y < maxVerticalSpeed && (nextGrounded || SnapToGround());

        if(nextGrounded) _stepSinceLastGrounded = 0;

        if(IsGrounded != realNextGrounded)
        {
            IsGrounded = realNextGrounded;
            if(realNextGrounded)
            {
                OnLandingInfos?.Invoke(this, new LandingEventArgs(-rb.velocity.y));
                OnLanding?.Invoke(this, EventArgs.Empty);
            }  
            else
                OnLeavingGround?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ResetJumpTracker()
    {
        _stepSinceLastJumped = 0;
    }

    bool SnapToGround()
    {
        if(_showLog)Debug.Log("Try snapping");
        if(_stepSinceLastGrounded > 1){
            if(_showLog) Debug.Log("Snap Aborted : Too late");
            return false;
        }
        if(!Physics.Raycast(spatial.position, Vector3.down, out RaycastHit snapHit))
        {
            if(_showLog) Debug.Log("Snap Aborted : No Ground Under");
            return false;
        }
        if(Vector3.Angle(Vector3.up, snapHit.normal) > groundedMaxAngle)
        {
            if(_showLog) Debug.Log("Snap Aborted : Angle to steep");
            return false;
        }
        if(_stepSinceLastJumped < _minJumpStep)
        {
            if(_showLog) Debug.Log("Snap Aborted : Jumped recently");
            return false;
        }

        GroundNormal = snapHit.normal;
        float speed = rb.velocity.magnitude;
        float dot = Vector3.Dot(rb.velocity, snapHit.normal);
        if(dot > 0f)
        {
            if(_showLog) Debug.Log("Snap Aborted : Wouldn't help snap");
            rb.velocity = (rb.velocity - snapHit.normal * dot).normalized * speed;
        }
        return true;
    }
}