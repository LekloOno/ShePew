using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_StepClimber : MonoBehaviour
{
    /*
    [SerializeField] bool isClimbing;
    
    void FixedUpdate()
    {
        isClimbing = false;
        if((inputHandler.WishDir != Vector3.zero || (rb.velocity.x != 0 || rb.velocity.z != 0)) && Physics.Raycast(lowerPoint, inputHandler.WishDir, out lowerHit, maxDistance, ground))
        {
            GroundNormal = lowerHit.normal;
            GroundAngle = Vector3.Angle(Vector3.up, lowerHit.normal);
            if(!Physics.Raycast(upperPoint, inputHandler.WishDir, out upperHit, maxDistance+upperDistanceOffset, ground) && GroundAngle > 45)
            {
                rb.position += new Vector3(0f, climbSpeed, 0f);
                GroundNormal = Vector3.up;
                GroundAngle = 0;
                UpdateGrounded(true);
                isClimbing = true;
            }
        }
    }*/
}
