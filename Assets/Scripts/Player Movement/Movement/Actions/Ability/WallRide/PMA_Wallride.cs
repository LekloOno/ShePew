using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Wallride : PMA_Ability<DATA_Wallride>
{
    public bool IsActive = false;
    public Vector3 WallLine;

    [Header("Specifics")]
    [SerializeField] Transform _playerDir;
    [SerializeField] Transform _camera;
    [SerializeField] PMA_AirControl _airHandler;
    [SerializeField] PMA_Slide _slideHandler;
    [SerializeField] LayerMask _ground;

    float _rideStartTime = Mathf.NegativeInfinity;
    float _kickStartTime = Mathf.NegativeInfinity;

    float _rideRealForce;
    float _kickRealForce;

    float _rideTracker;

    bool _wasSliding = false;
    Collider _currentCollider;
    RaycastHit _rightWall;
    RaycastHit _leftWall;
    RaycastHit _wall;
    Vector3 _wallNormal;

    [Header("Tweaking")]

    [SerializeField] float _kickOutMultiplier;
    [SerializeField] float _kickInMultiplier;
    [SerializeField] float _kickUpMultiplier;

    #region Setup

    protected override void Start()
    {
        action = inputMapsManager.playerInputActions.Arena.Jump;
        base.Start();
    }

    #endregion

    public override void StartAbility(InputAction.CallbackContext obj)
    {
        if(!_groundState.IsGrounded && (Physics.Raycast(rb.transform.position, _playerDir.right, out _wall, data.WallMaxDistance, _ground) || Physics.Raycast(rb.transform.position, -_playerDir.right, out _wall, data.WallMaxDistance, _ground)))
        {
            float wallAngle = Vector3.Angle(Vector3.up, _wall.normal);
            if(wallAngle < data.WallMaxAngle && wallAngle > data.WallMinAngle)
            {
                if(_currentCollider == _wall.collider)
                {
                    _rideRealForce = Mathf.Pow(Mathf.Min(data.WallBoostRecovery, Time.time-_rideStartTime)/data.WallBoostRecovery,data.WallBoostRecoveryStrength);
                }
                else
                {
                    _rideRealForce = 1;
                    _kickStartTime = Mathf.NegativeInfinity;
                }
                _wasSliding = _slideHandler.IsActive;
                if(_wasSliding)
                {
                    _slideHandler.StopSlide();
                }

                _currentCollider = _wall.collider;
                _rideStartTime = Time.time;
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                IsActive = true;
                _airHandler.enabled = false;
                _rideTracker = data.WallRideLength;

                OnFixedUpdate += OnWallRide;
            }    
        }
    }

    public override void StopAbility(InputAction.CallbackContext obj)
    {
        if(IsActive)
        {
            StopWallRide(true);
        }
    }

    void StopWallRide(bool cancel)
    {
        _kickRealForce = Mathf.Pow(Mathf.Min((data.WallBoostRecovery), Time.time-_kickStartTime)/(data.WallBoostRecovery),2);
        _kickStartTime = Time.time;
        IsActive = false;
        OnFixedUpdate -= OnWallRide;
        if(cancel)     
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Vector3 kickDir = Vector3.Lerp((WallLine*_kickInMultiplier+_wallNormal*_kickOutMultiplier+Vector3.up*_kickUpMultiplier).normalized, _camera.forward.normalized, data.WallKickControl);
            rb.AddForce(kickDir*data.WallKickStrength*_kickRealForce, ForceMode.Impulse);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x*0.4f, 0.2f*rb.velocity.y, rb.velocity.z*0.4f);
            rb.AddForce(_wallNormal.normalized*data.WallKickStrength*_kickRealForce, ForceMode.Impulse);
        }
        
        if(_wasSliding)
            _slideHandler.InitiateSlide();
        else
            _airHandler.enabled = true;
            
        rb.useGravity = true;
    }

    void OnWallRide(object sender, EventArgs e)
    {
        Vector3 wallClosestPoint = _currentCollider.ClosestPoint(rb.transform.position);

        if(_rideTracker > 0 && !_groundState.IsGrounded && (rb.transform.position - wallClosestPoint).magnitude < data.WallMaxDistance)
        {
            float flatTimeSpent = (data.WallRideLength-_rideTracker)/data.WallRideLength;
            _wallNormal = rb.transform.position - wallClosestPoint;
            WallLine = Vector3.Cross(_wallNormal, Vector3.up).normalized;
            Vector3 localWishDir;
            if(inputHandler.RunningAxis.y == 0)
                localWishDir = _playerDir.forward * 1 + _playerDir.right * inputHandler.RunningAxis.x;
            else
                localWishDir = inputHandler.WishDir;
            localWishDir = (Vector3.Dot(WallLine, localWishDir)*WallLine).normalized;
            rb.AddForce(Mathf.Max(Mathf.Min(_rideRealForce*data.RideMaxSpeed - Vector3.Dot(localWishDir, rb.velocity), data.RideMaxAccel*_rideRealForce*data.RideMaxSpeed*Time.fixedDeltaTime),0)*localWishDir, ForceMode.VelocityChange);
            rb.AddForce(Physics.gravity * Mathf.Pow(flatTimeSpent,1.5f) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            _rideTracker -= Time.fixedDeltaTime;
        }
        else
        {
            StopWallRide(false);    
        }
    }
}
