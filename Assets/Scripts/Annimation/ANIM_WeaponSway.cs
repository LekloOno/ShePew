using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIM_WeaponSway : MonoBehaviour
{
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PIA_RunningProcessing _runningInput;
    
    [Header("Keyboard Sway")]
    [SerializeField] private float _minSpeedTrigger;
    [SerializeField] private float _keyRotationStrength;
    [SerializeField] private float _keyMaxPosition;

    [Header("Mouse Sway")]
    [SerializeField] private PC_Control _cameraControl;
    [SerializeField] private float _reactive;
    [SerializeField] private float _strength;
    [SerializeField] private float _mouseMaxPosition;

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
            movePosition = new Vector3(-_runningInput.RunningAxis.x * _keyMaxPosition, 0, -_runningInput.RunningAxis.y * _keyMaxPosition); 
        }
        else
        {
            moveRotationX = Quaternion.identity;
            moveRotationZ = Quaternion.identity;
        }

        Quaternion rotationX = Quaternion.AngleAxis(-_cameraControl.LookY * _strength, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(_cameraControl.LookX * _strength, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(_cameraControl.LookX * _strength, -Vector3.forward);
        mousePosition = new Vector3(_cameraControl.LookX*_mouseMaxPosition, 0, _cameraControl.LookY*_mouseMaxPosition);

        Quaternion targetRotation = rotationX * rotationY * rotationZ * moveRotationX * moveRotationZ;

        Vector3 targetPosition = mousePosition + movePosition;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _reactive*Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, _reactive*Time.deltaTime);
    }
}
