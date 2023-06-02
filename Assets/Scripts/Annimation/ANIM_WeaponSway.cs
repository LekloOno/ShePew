using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIM_WeaponSway : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private PC_Control _cameraControl;
    [SerializeField] private float _reactive;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PIA_RunningProcessing _runningInput;
    [SerializeField] private float _maxPosition;
    [SerializeField] private float _maxRotation;
    
    [Header("Keyboard Sway")]
    [SerializeField] private float _minSpeedTrigger;
    [SerializeField] private float _keyRotationStrength;
    [SerializeField] private float _keyPositionStrength;

    [Header("Mouse Sway")]
    [SerializeField] private float _mouseRotationStrength;
    [SerializeField] private float _mousePositionStrength;

    void Update()
    {
        Vector3 movePosition = Vector3.zero;
        Vector3 mousePosition = Vector3.zero;

        Quaternion moveRotationX;
        Quaternion moveRotationZ;
        if(_groundState.IsGrounded && _groundState.FlatSpeed >= _minSpeedTrigger)
        {
            moveRotationX = Quaternion.AngleAxis(_runningInput.RunningAxis.x * _keyRotationStrength, Vector3.up);
            moveRotationZ = Quaternion.AngleAxis(_runningInput.RunningAxis.x * _keyRotationStrength, -Vector3.forward);
            movePosition = new Vector3(-_runningInput.RunningAxis.x * _keyPositionStrength, 0, -_runningInput.RunningAxis.y * _keyPositionStrength); 
        }
        else
        {
            moveRotationX = Quaternion.identity;
            moveRotationZ = Quaternion.identity;
        }

        Quaternion rotationX = Quaternion.AngleAxis(-_cameraControl.LookY * _mouseRotationStrength, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(_cameraControl.LookX * _mouseRotationStrength, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(_cameraControl.LookX * _mouseRotationStrength, -Vector3.forward);
        mousePosition = new Vector3(-_cameraControl.LookX*_mousePositionStrength, -_cameraControl.LookY*_mousePositionStrength, 0);

        float _xRotation = Mathf.Clamp(-_cameraControl.LookY * _mouseRotationStrength, -_maxRotation, _maxRotation);
        float _yRotation = Mathf.Clamp(_cameraControl.LookX * _mouseRotationStrength + _runningInput.RunningAxis.x * _keyRotationStrength, -_maxRotation, _maxRotation);
        float _zRotation = Mathf.Clamp(_cameraControl.LookX * _mouseRotationStrength + _runningInput.RunningAxis.x * _keyRotationStrength, -_maxRotation/2, _maxRotation/2);

        //_sightPosition.rotation = Quaternion.Euler(_xRotation, _yRotation, ZRotation);
        //_flatDir.rotation = Quaternion.Euler(0, _yRotation, ZRotation);

        //Quaternion targetRotation = rotationX * rotationY * rotationZ * moveRotationX * moveRotationZ;
        Quaternion targetRotation = Quaternion.Euler(_xRotation, _yRotation, _zRotation);
        //Debug.Log(targetRotation.eulerAngles);

        //Vector3 targetPosition = Mathf.Min(mousePosition + movePosition, _maxPosition);
        Vector3 targetPosition = MaxSwayPosition(mousePosition + movePosition);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _reactive*Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, _reactive*Time.deltaTime);
    }

    Vector3 MaxSwayPosition(Vector3 v)
    {
        Vector3 abs = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        Vector3 maxed = new Vector3(Mathf.Min(abs.x, _maxPosition), Mathf.Min(abs.y, _maxPosition), Mathf.Min(abs.z, _maxPosition));
        return new Vector3(Mathf.Sign(v.x)*maxed.x, Mathf.Sign(v.y)*maxed.y, Mathf.Sign(v.z)*maxed.z);
    }
}
