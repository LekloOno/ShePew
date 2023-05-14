using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PC_SlideInBobbing : MonoBehaviour
{
    [SerializeField] private PMA_Slide _slide;
    [SerializeField] private Transform _slideBobbingAnchor;
    [SerializeField] private AnimationCurve _slideCurve;
    [SerializeField, Range(0, 1f)] private float _slideDuration;
    [SerializeField, Range(-0.5f, 0.5f)] private float _slideAmplitudeX;
    [SerializeField, Range(0, 0.5f)] private float _slideAmplitudeY;

    private float _initTime;
    private Vector3 _startPos;

    private bool _slidingIn;

    private void Awake()
    {
        _startPos = _slideBobbingAnchor.localPosition;
    }

    public void Start()
    {
        _slide.OnInputIn += SlideInBobbing_OnInputIn;
    }


    public void SlideInBobbing_OnInputIn(object sender, EventArgs e)
    {
        if(true)
        {
            _initTime = Time.time;
            _slidingIn = true;
        }
    }

    private Vector3 SlideMotion(float time)
    {
        Vector3 pos = Vector3.zero;
        pos.x = _slideCurve.Evaluate(time) * _slideAmplitudeX;
        pos.y = -_slideCurve.Evaluate(time) * _slideAmplitudeY;
        return pos;
    }

    private void CheckSlideInMotion()
    {
        float currentTime = (Time.time-_initTime)/_slideDuration;
        if(currentTime<1)
        {
            _slideBobbingAnchor.localPosition = SlideMotion(currentTime);
        }
        else
        {
            _slidingIn = false;
        }
    }

    private void PlayMotion(Vector3 motion)
    {
        _slideBobbingAnchor.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (_slideBobbingAnchor.localPosition == _startPos) return;
        _slideBobbingAnchor.localPosition = Vector3.Lerp(_slideBobbingAnchor.localPosition, _startPos, 1 * Time.deltaTime);
    }

    void Update()
    {
        if(_slidingIn)
            CheckSlideInMotion();
        else
            ResetPosition();
    }
}
