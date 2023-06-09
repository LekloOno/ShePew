using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(30)]
public class PC_Control : MonoBehaviour
{
    public float ZRotation = 0;

    public float Sens;
    public float X_Sens;
    public float Y_Sens;

    private float _lookX;
    private float _lookY;
    
    public float LookX {get => _lookX;}
    public float LookY {get => _lookY;}

    [SerializeField] Transform _sightPosition;
    [SerializeField] Transform _cameraPosition;
    [SerializeField] Transform _flatDir;
    [SerializeField] PI_AMapsManager _inputHandler;
    [SerializeField] float _globalSensAdjustment;


    public Transform FlatDir
    {
        get => _flatDir;
    }

    float _xRotation;
    float _yRotation;


    public event EventHandler ApplyCameraEffects;

    void Update()
    {
        OnLook();
        transform.position = _sightPosition.position;
        transform.rotation = _sightPosition.rotation;
        ApplyCameraEffects?.Invoke(this, EventArgs.Empty);
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook()
    {
        _lookX = _inputHandler.playerInputActions.Arena.Look.ReadValue<Vector2>().x*_globalSensAdjustment*(X_Sens+Sens);
        _lookY = _inputHandler.playerInputActions.Arena.Look.ReadValue<Vector2>().y*_globalSensAdjustment*(Y_Sens+Sens);

        _xRotation = Mathf.Clamp(_xRotation-_lookY, -90f, 90f);

        _yRotation += _lookX;

        _sightPosition.rotation = Quaternion.Euler(_xRotation, _yRotation, ZRotation);
        _flatDir.rotation = Quaternion.Euler(0, _yRotation, ZRotation);
    }
}
