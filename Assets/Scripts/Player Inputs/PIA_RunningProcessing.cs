using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script Processes the Arena Action map running inputs with the flatDir into a WishDir,
so that each "WASD" horizontal movements scripts don't have to redo this quick operation.

It also stores other general informations about these "WASD" inputs.
*/
public class PIA_RunningProcessing : MonoBehaviour
{
    public Vector3 SpaceWishDir;
    public Vector2 WishDir;
    public Vector2 RunningAxis;
    public float LastBackward = Mathf.NegativeInfinity;

    [SerializeField] private PI_AMapsManager inputsMM;

    [SerializeField] Transform flatDir;
    [SerializeField] Transform sightPosition;

    void FixedUpdate()
    {
        RunningAxis = inputsMM.playerInputActions.Arena.Running.ReadValue<Vector2>();
        if(RunningAxis.y < 0)
        {
            LastBackward = Time.time;
        }
        WishDir = flatDir.forward * RunningAxis.y + flatDir.right * RunningAxis.x;
        SpaceWishDir = sightPosition.forward * RunningAxis.y + sightPosition.right * RunningAxis.x;
    }
}
