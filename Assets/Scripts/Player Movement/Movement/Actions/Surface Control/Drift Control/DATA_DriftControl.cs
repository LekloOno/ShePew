using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_DriftControl", menuName = "Player Movement/Actions/Drift Control Data")]
public class DATA_DriftControl : DATA_SurfaceControl
{
    public PhysicMaterial Mat;              //Physic Material used when drifting
    [Header("Specifics")]
    public float MaxHeight = 1.5f;          //Optimal Jump Height
    public float MinDuration = 0.5f;        //Minimum duration of the drift
    public float MaxDuration = 1;           // SHOULD BE BIGGER THAN MinDuration - Maximum duration of the drift if the player fell from MaxHeight 
    public float RealMaxDuration = 1.1f;    // SHOULD BE BIGGER THAN MaxDuration - Limit of the duration for an infinite downward velocity.
    public float DurationStrength = 1;      //Tension of the MinDuration to MaxDuration curve
    public float TractionDrag = 2.5f;       //Drag applied by the traction pivot
    public float BackBuffer = 0.2f;         //Maximum time between the last backward input and the slide input for the drift to be started 
}
