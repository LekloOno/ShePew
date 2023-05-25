using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIM_WeaponSway : MonoBehaviour
{
    [SerializeField] private PC_Control _cameraControl;
    [SerializeField] private float _reactive;
    [SerializeField] private float _strength;

    void Update()
    {
        Quaternion rotationX = Quaternion.AngleAxis(-_cameraControl.LookY * _strength, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(_cameraControl.LookX * _strength, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(_cameraControl.LookX * _strength, -Vector3.forward);

        Quaternion targetRotation = rotationX * rotationY * rotationZ;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _reactive*Time.deltaTime);
    }
}
