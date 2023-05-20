using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PC_Bobbing : MonoBehaviour
{
    [SerializeField] private bool _enable = false;

    public bool Enable
    {
        set => _enable = value;
    }

    [SerializeField, Range(0, 0.01f)] private float _sprintAmplitude = 0.0007f;
    [SerializeField, Range(0, 1)] private float _sprintWaveLength = 0.15f;
    [SerializeField, Range(0, 0.01f)] private float _walkAmplitude = 0.0004f;
    [SerializeField, Range(0, 1)] private float _walkWaveLength = 0.25f;

    private float _amplitude;
    private float _waveLength;

    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PMA_GroundControlManager _groundControl;

    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = _camera.localPosition;
        _groundControl.StartSprint += OnStartSprint;
        _groundControl.StopSprint += OnStopSprint;
        _groundControl.StartWalking += OnStartWalking;
        _groundControl.StopWalking += OnStopWalking;
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _amplitude = _sprintAmplitude;
        _waveLength = _sprintWaveLength;
        _enable = true;
    }

    public void OnStopSprint(object sender, EventArgs e)
    {
        _enable = false;
    }

    public void OnStartWalking(object sender, EventArgs e)
    {
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;
        _enable = true;
    }

    public void OnStopWalking(object sender, EventArgs e)
    {
        _enable = false;
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin((Time.time/_waveLength) * Mathf.PI) * _amplitude;
        pos.x += Mathf.Sin((Time.time/_waveLength) * Mathf.PI / 2) * _amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        if (_groundState.FlatSpeed < _toggleSpeed) return;
        if (!_groundState.IsGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    void Update()
    {
        if (!_enable) ResetPosition();
        else {
            CheckMotion();
            ResetPosition();
        }
        //_camera.LookAt(FocusTarget());
    }
/*
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }*/
}
