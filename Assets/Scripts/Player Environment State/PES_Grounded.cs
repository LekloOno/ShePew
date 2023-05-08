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
    public event EventHandler OnLeavingGround;

    [Header("Rigidbody")]
    public float FlatVelocity;
    public Vector3 Velocity;
    
    [Header("Tweak")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform spatial;
    [SerializeField] Transform body;
    [SerializeField] LayerMask ground;
    [SerializeField] float distToGround;
    [SerializeField] float groundedRadius;
    [SerializeField] float groundedMaxAngle;
    [SerializeField] float maxVerticalSpeed;

    RaycastHit groundHit;
    RaycastHit groundedHit;

    float groundInfoRadius;
    float realDistToGround;

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
        realDistToGround = transform.localScale.y*distToGround;
        Velocity = rb.velocity;
        FlatVelocity = (new Vector3(Velocity.x,0,Velocity.z)).magnitude;

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

    void UpdateGrounded(bool nextGrounded)
    {
        nextGrounded = nextGrounded && rb.velocity.y < maxVerticalSpeed;
        if(IsGrounded != nextGrounded)
        {
            IsGrounded = nextGrounded;

            if(nextGrounded)
                OnLanding?.Invoke(this, EventArgs.Empty);
            else
                OnLeavingGround?.Invoke(this, EventArgs.Empty);
        }
    }
}