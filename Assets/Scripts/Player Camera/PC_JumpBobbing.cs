using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PC_JumpBobbing : MonoBehaviour
{
    [SerializeField] private PMA_Jump _jump;
    [SerializeField] private Transform _jumpBobbingAnchor;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField, Range(0, 1f)] private float _jumpDuration;
    [SerializeField, Range(0, 2f)] private float _jumpAmplitudeX;
    [SerializeField, Range(0, 2f)] private float _jumpAmplitudeY;

    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private AnimationCurve _landCurve;
    [SerializeField] private AnimationCurve _speedCurveImpact;
    [SerializeField, Range(0, 1f)] private float _landDuration;
    [SerializeField, Range(0, 2f)] private float _landAmplitudeX;
    [SerializeField, Range(0, 2f)] private float _landAmplitudeY;
    [SerializeField, Range(0, 20f)] private float _maxSpeed;

    private float _initTime;
    private float _forceAmplitude;
    private float _speedAmplitude;
    private Vector3 _startPos;
    

    private bool _jumping;
    private bool _landing;
    
    private void Awake()
    {
        _startPos = _jumpBobbingAnchor.localPosition;
    }

    public void Start()
    {
        _jump.OnJumped += JumpBobbing_OnJumped;
        _groundState.OnLandingInfos += JumpBobbing_OnLanded;
    }

    public void JumpBobbing_OnJumped(object sender, JumpEventArgs e)
    {
        _forceAmplitude = e.Force;
        _initTime = Time.time;
        _landing = false;
        _jumping = true;
    }

    public void JumpBobbing_OnLanded(object sender, LandingEventArgs e){
        _speedAmplitude = _speedCurveImpact.Evaluate(Mathf.Min(e.Speed, _maxSpeed)/_maxSpeed);
        _initTime = Time.time;
        _jumping = false;
        _landing = true;
    }

    private Vector3 JumpStepMotion(float time)
    {
        Vector3 pos = Vector3.zero;
        pos.x = _jumpCurve.Evaluate(time) * _jumpAmplitudeX * _forceAmplitude;
        pos.y = _jumpCurve.Evaluate(time) * _jumpAmplitudeY * _forceAmplitude;
        return pos;
    }

    private Vector3 LandStepMotion(float time)
    {
        Vector3 pos = Vector3.zero;
        pos.x = -_landCurve.Evaluate(time) * _landAmplitudeX * _speedAmplitude;
        pos.y = -_landCurve.Evaluate(time) * _landAmplitudeY * _speedAmplitude;
        return pos;
    }

    private void CheckJumpMotion()
    {
        float currentTime = (Time.time-_initTime)/_jumpDuration;
        if(currentTime<1)
        {
            _jumpBobbingAnchor.localPosition = JumpStepMotion(currentTime);
        }
        else
        {
            _jumping = false;
        }
    }

    private void CheckLandMotion()
    {
        float currentTime = (Time.time-_initTime)/_jumpDuration;
        if(currentTime<1)
        {
            _jumpBobbingAnchor.localPosition = LandStepMotion(currentTime);
        }
        else
        {
            _landing = false;
        }
    }

    private void PlayMotion(Vector3 motion)
    {
        _jumpBobbingAnchor.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (_jumpBobbingAnchor.localPosition == _startPos) return;
        _jumpBobbingAnchor.localPosition = Vector3.Lerp(_jumpBobbingAnchor.localPosition, _startPos, 1 * Time.deltaTime);
    }

    void Update()
    {
        if(_landing)
            CheckLandMotion();
        else if(_jumping)
            CheckJumpMotion();
        else
            ResetPosition();
        //_jumpBobbingAnchor.LookAt(FocusTarget());
    }
}
