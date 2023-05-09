using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_GroundControl", menuName = "Player Movement/Actions/Ground Control Data")]
public class DATA_GroundControl : DATA_SurfaceControl
{
    public PhysicMaterial Mat;      //Physic material used for the groundcontrol behavior
}
