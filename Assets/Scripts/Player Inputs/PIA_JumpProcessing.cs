using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[DefaultExecutionOrder(48)]
public class PIA_JumpProcessing : MonoBehaviour
{
    [SerializeField] private PI_AMapsManager _inputMapsManager;
    [SerializeField] private float _bufferWindow = 0.03f;
    private float _stepSinceLastJumped = Mathf.Infinity;
    private bool _jumpBuffered = false;
    public bool JumpBuffered{get=>_jumpBuffered;}
    public float StepSinceLastJumped {get => _stepSinceLastJumped;}

    void Start()
    {
        _inputMapsManager.playerInputActions.Arena.Jump.performed += OnJumpDown;
    }

    void FixedUpdate()
    {
        _stepSinceLastJumped ++;
    }

    public void ResetJumpTracker()
    {
        _stepSinceLastJumped = 0;
    }

    public void OnJumpDown(InputAction.CallbackContext obj)
    {
        CancelInvoke("ClearBuffer");
        _jumpBuffered = true;
        Invoke("ClearBuffer", _bufferWindow);
    }

    private void ClearBuffer()
    {
        _jumpBuffered = false;
    }

    public bool UseBuffer()
    {
        bool wasBuffered = _jumpBuffered;
        CancelInvoke("ClearBuffer");
        ClearBuffer();
        return wasBuffered;
    }
}
