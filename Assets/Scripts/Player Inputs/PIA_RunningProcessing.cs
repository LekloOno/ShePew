using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
This script Processes the Arena Action map running inputs with the flatDir into a WishDir,
so that each "WASD" horizontal movements scripts don't have to redo this quick operation.

It also stores other general informations about these "WASD" inputs.
*/
public class PIA_RunningProcessing : MonoBehaviour
{
    public EventHandler OnStopOrLess;
    public EventHandler<Vector2> KeyPressed;
    public Vector3 SpaceWishDir;
    public Vector3 WishDir;
    public Vector2 RunningAxis;
    public Vector2 nextRunningAxis;
    public float LastBackward = Mathf.NegativeInfinity;

    [SerializeField] private PI_AMapsManager inputsMM;

    [SerializeField] Transform flatDir;
    [SerializeField] Transform sightPosition;


    void FixedUpdate()
    {
        nextRunningAxis = inputsMM.playerInputActions.Arena.Running.ReadValue<Vector2>();
        if(nextRunningAxis != Vector2.zero && nextRunningAxis != RunningAxis)
        {
            KeyPressed?.Invoke(this, nextRunningAxis);
        }
        RunningAxis = nextRunningAxis;
        if(RunningAxis.y < 0)
        {
            LastBackward = Time.time;
        }
        if(RunningAxis.y < 0 || (RunningAxis.x == 0 && RunningAxis.y == 0))
        {
            OnStopOrLess?.Invoke(this, EventArgs.Empty);
        }
        WishDir = flatDir.forward * RunningAxis.y + flatDir.right * RunningAxis.x;
        SpaceWishDir = sightPosition.forward * RunningAxis.y + sightPosition.right * RunningAxis.x;
    }

    public Vector3 FreeWishDir(Vector2 input)
    {
        return flatDir.forward * input.y + flatDir.right * input.x;
    }
}
