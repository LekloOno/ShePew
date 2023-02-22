using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PSS_Ground : MonoBehaviour
{
    [Header("States")]
    public bool IsGrounded;
    public bool IsSloped;
    public Collider GroundCollider;
    public float GroundHeight;
    public float GroundAngle;
    public Vector3 GroundNormal;

    public event EventHandler OnLanding;
    public event EventHandler OnLeavingGround;
    public event EventHandler OnEnteringSlope;
    public event EventHandler OnExitingSlope;
    public event EventHandler OnUpdatingGround;
    public event EventHandler OnUpdatingGroundCollider;

    [Header("Rigidbody")]
    public float FlatVelocity;
    public Vector3 Velocity;
    
    [Header("Tweak")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform spatial;
    [SerializeField] Transform body;
    [SerializeField] Transform lowerSphere;
    [SerializeField] PIS_Combat inputHandler;
    [SerializeField] LayerMask ground;
    [SerializeField] float distToGround;
    [SerializeField] float groundedRadius;
    [SerializeField] float groundedMaxAngle;
    //[SerializeField] float distTGAngleMultiplier;
    [Header("Tweak/climber")]
    [SerializeField] float climbSpeed;
    [SerializeField] float floorMargin;
    [SerializeField] float maxHeight;
    [SerializeField] float maxDistance;
    [SerializeField] float upperDistanceOffset;
    [Header("Tweak/Slope")]
    [SerializeField] float slopeMinAngle;
    [SerializeField] float slopeMaxAngle;
    [SerializeField] float maxVerticalSpeed;

    RaycastHit groundHit;
    RaycastHit groundedHit;
    float finalDistTG;

    RaycastHit lowerHit;
    RaycastHit upperHit;
    Vector3 lowerPoint;
    Vector3 upperPoint;

    [SerializeField] bool isClimbing;

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
        //if(rb.velocity.y<0) Debug.Log(rb.velocity.y + " | ");
        realDistToGround = transform.localScale.y*distToGround;
        Velocity = rb.velocity;
        FlatVelocity = (new Vector3(Velocity.x,0,Velocity.z)).magnitude;

        if(Physics.SphereCast(spatial.position, groundInfoRadius, Vector3.down, out groundHit, Mathf.Infinity, ground))
        {
            GroundHeight = groundHit.distance+groundInfoRadius;
            if(GroundCollider != groundHit.collider)
            {
                OnUpdatingGroundCollider?.Invoke(this,EventArgs.Empty);
                GroundCollider = groundHit.collider;
            }
            OnUpdatingGround?.Invoke(this, EventArgs.Empty);
        }

        lowerPoint = new Vector3(spatial.position.x, spatial.position.y - 0.9f*transform.localScale.y+floorMargin, spatial.position.z);
        upperPoint = new Vector3(spatial.position.x, spatial.position.y - 0.9f*transform.localScale.y+maxHeight, spatial.position.z);

        isClimbing = false;
        if((inputHandler.WishDir != Vector3.zero || (rb.velocity.x != 0 || rb.velocity.z != 0)) && Physics.Raycast(lowerPoint, inputHandler.WishDir, out lowerHit, maxDistance, ground))
        {
            GroundNormal = lowerHit.normal;
            GroundAngle = Vector3.Angle(Vector3.up, lowerHit.normal);
            if(!Physics.Raycast(upperPoint, inputHandler.WishDir, out upperHit, maxDistance+upperDistanceOffset, ground) && GroundAngle > 45)
            {
                rb.position += new Vector3(0f, climbSpeed, 0f);
                GroundNormal = Vector3.up;
                GroundAngle = 0;
                UpdateGrounded(true);
                isClimbing = true;
            }
        }
        
        if(!isClimbing)
        {
            if(Physics.SphereCast(spatial.position, groundedRadius, Vector3.down, out groundedHit, realDistToGround-groundedRadius, ground))
            {
                GroundNormal = groundedHit.normal;
                GroundAngle = Vector3.Angle(Vector3.up, groundedHit.normal);
                GroundCollider = groundedHit.collider;
                UpdateGrounded(GroundAngle<=groundedMaxAngle);
            }
            else
            {
                GroundNormal = Vector3.up;
                GroundAngle = 0;
                UpdateGrounded(false);
            }
        }

        UpdateSloped(IsGrounded && GroundAngle <= slopeMaxAngle && GroundAngle >= slopeMinAngle);

        
        #region Obsolete
        /*
        /PREVIOUS RAY CASTING GROUND DETECTION\


        if(Physics.Raycast(transform.position, Vector3.down, out groundHit, Mathf.Infinity, ground))
        {
            GroundHeight = groundHit.distance;
            //GroundCollider = groundHit.collider;
            if(groundHit.distance <= finalDistTG+0.3f)
            {
                lowerPoint = new Vector3(rb.position.x, rb.position.y - 0.9f+floorMargin, rb.position.z);
                upperPoint = new Vector3(rb.position.x, rb.position.y - 0.9f+maxHeight, rb.position.z);

                if(inputHandler.WishDir != Vector3.zero && Physics.Raycast(lowerPoint, inputHandler.WishDir, out lowerHit, maxDistance, ground))
                {
                    GroundNormal = lowerHit.normal;
                    GroundAngle = Vector3.Angle(Vector3.up, lowerHit.normal);
                    if(!Physics.Raycast(upperPoint, inputHandler.WishDir, out upperHit, maxDistance+upperDistanceOffset, ground) && GroundAngle > 45 && IsGrounded)
                    {
                        rb.position += new Vector3(0f, climbSpeed, 0f);
                        GroundNormal = Vector3.up;
                        GroundAngle = 0;
                    }
                }
                else
                {
                    GroundNormal = groundHit.normal;
                    GroundAngle = Vector3.Angle(Vector3.up, groundHit.normal);
                }
            }
            else
            {
                GroundNormal = Vector3.up;
                GroundAngle = 0;
            }
            
            
            finalDistTG = distToGround+Mathf.Min(GroundAngle,45)*distTGAngleMultiplier;

            bool nextGrounded = groundHit.distance <= finalDistTG;

            if(IsGrounded != nextGrounded)
            {
                IsGrounded = nextGrounded;

                if(nextGrounded)
                    OnLanding?.Invoke(this, EventArgs.Empty);
                else
                    OnLeavingGround?.Invoke(this, EventArgs.Empty);
            }
        }*/
        #endregion
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

    void UpdateSloped(bool nextSloped)
    {
        if(IsSloped != nextSloped)
        {
            IsSloped = nextSloped;
            if(nextSloped)
                OnEnteringSlope?.Invoke(this, EventArgs.Empty);
            else
                OnExitingSlope?.Invoke(this, EventArgs.Empty);
        }
    }
}
