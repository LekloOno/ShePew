using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_SlideTilt : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PC_Control _cam;
    [SerializeField] private PMA_Slide _slide;
    [SerializeField] private float _slideTiltingAngle;
    [SerializeField] private float _slideTiltingSpeed;
    [SerializeField] private float _slideTiltingResetSpeed;

    void Update()
    {
        if(_slide.IsActive)
            slideTilting(_rb.velocity.normalized, _cam.FlatDir.transform.TransformDirection(Vector3.right).normalized);
        else if(_cam.ZRotation != 0)
            resetSlideTilting();
    }

    public void slideTilting(Vector3 velocity, Vector3 fwdDir, float ratio=1)
    {
        float velDirScalar = Vector3.Dot(velocity, fwdDir);
        _cam.ZRotation += (_slideTiltingAngle*ratio*velDirScalar-_cam.ZRotation)*Time.deltaTime*_slideTiltingSpeed;
    }

    public void resetSlideTilting()
    {
        _cam.ZRotation += (-_cam.ZRotation)*Time.deltaTime*_slideTiltingResetSpeed;
    }
}
