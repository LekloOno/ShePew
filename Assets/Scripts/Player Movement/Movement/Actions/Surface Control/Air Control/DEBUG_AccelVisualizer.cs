using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEBUG_AccelVisualizer : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PM_DataContainer dataContainer;
    [SerializeField] private Camera _camera;
    [SerializeField] private PIA_RunningProcessing inputHandler;
    [SerializeField] private Image image;
    public float Angle;
    public float AngleDiff;

    void FixedUpdate(){
        Vector3 projected = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        Angle = MovementPhysics.MaxAccelAngle(rb.velocity.magnitude, dataContainer.Data.MaxSpeed, dataContainer.Data.MaxAccel);
        Vector3 right = Quaternion.AngleAxis(Angle, Vector3.up) * projected;
        Vector3 left = Quaternion.AngleAxis(-Angle, Vector3.up) * projected;

        Vector3 closest = Mathf.Abs(Vector3.Angle(inputHandler.WishDir, right)) > Mathf.Abs(Vector3.Angle(inputHandler.WishDir, left)) ? left : right;
        AngleDiff = (Angle - Vector3.Angle(projected, inputHandler.WishDir)) * (closest == left ? -1 : 1);
        Vector3 camClosest = Quaternion.AngleAxis(AngleDiff, Vector3.up) * new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z);

        Debug.Log(camClosest + " | " + _camera.transform.forward);

        image.transform.position = _camera.WorldToScreenPoint(_camera.transform.position - camClosest);
    }
}
