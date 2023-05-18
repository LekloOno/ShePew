using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_StepClimber : MonoBehaviour
{
    [SerializeField] Transform spatial;
    Vector3 lowerPoint;
    Vector3 upperPoint;
    [SerializeField] PMA_GroundControlManager _groundControlManager;
    [SerializeField] PM_SC_Manager _surfaceControlManager;
    [SerializeField] PES_Grounded _groundState;
    [SerializeField] bool isClimbing;
    [SerializeField] float climbSpeed = 0.02f;
    [SerializeField] float maxDistance = 0.5f;
    [SerializeField] float upperDistanceOffset = 0f;
    [SerializeField] float floorMargin = 0.05f;
    [SerializeField] float maxHeight = 0.3f;
    [SerializeField] Rigidbody rb;
    [SerializeField] PIA_RunningProcessing inputHandler;

    RaycastHit lowerHit;
    RaycastHit upperHit;
    
    void FixedUpdate()
    {
        lowerPoint = new Vector3(spatial.position.x, spatial.position.y - 0.9f*transform.localScale.y+floorMargin, spatial.position.z);
        upperPoint = new Vector3(spatial.position.x, spatial.position.y - 0.9f*transform.localScale.y+maxHeight, spatial.position.z);

        isClimbing = false;
        if((inputHandler.WishDir != Vector3.zero || (rb.velocity.x != 0 || rb.velocity.z != 0)) && Physics.Raycast(lowerPoint, inputHandler.WishDir, out lowerHit, maxDistance, _groundState.Ground))
        {
            _groundState.GroundNormal = lowerHit.normal;
            _groundState.GroundAngle = Vector3.Angle(Vector3.up, lowerHit.normal);
            if(!Physics.Raycast(upperPoint, inputHandler.WishDir, out upperHit, maxDistance+upperDistanceOffset, _groundState.Ground) && _groundState.GroundAngle > 45)
            {
                rb.position += new Vector3(0f, climbSpeed, 0f);
                _groundState.GroundNormal = Vector3.up;
                _groundState.GroundAngle = 0;
                _groundState.UpdateGrounded(true);
                isClimbing = true;
            }
        }

        if(isClimbing)
        {
            //_surfaceControlManager.enabled = false;
        }
        else
        {
            //_surfaceControlManager.enabled = true;
        }
    }
}
