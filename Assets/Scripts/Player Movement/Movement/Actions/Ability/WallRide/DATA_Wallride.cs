using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_Wallride", menuName = "Player Movement/Actions/Wall Ride Data")]
public class DATA_Wallride : DATA_BaseAction
{
    [Header("Ride")]
    public float WallRideLength = 1.5f;
    public float RideMaxSpeed = 9;              //Will not gain speed from the wall ride beyond this speed
    public float RideMaxAccel = 5;

    //Wall Kick is performed by cancelling the Wall Ride
    [Header("Kick")]
    public float WallKickStrength = 5;          //The max strength of the kick
    public float WallKickControl = 0.2f;        //Between 0 and 1, What influence has the camera direction on the the kick direction. 1 = Total, 0 = None

    //If the player wall ride on the same wall twice, a decay is applied on the ride and kick strength.
    [Header("Decay")] 
    public float WallBoostRecovery = 3;         //How much time it takes for the Wall boost to be completely reseted
    public float WallBoostRecoveryStrength = 2; //The curve tension of the recovery.
    
    [Header("Wall Condition")]
    public float WallMaxDistance = 1.0f;        //Max distance to the wall to enable wall ride          - |-----(player)
    public float WallMinAngle = 75;             //Minimum angle for the surface to be considered a wall - \     (player)
    public float WallMaxAngle = 95;             //Maximum angle for the surface to be a climbable wall  - /     (player)
}
