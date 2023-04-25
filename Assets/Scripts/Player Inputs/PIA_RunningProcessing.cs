using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script Processes the Arena Action map running inputs with the playerDir into a WishDir,
so that each "WASD" horizontal movements scripts don't have to redo this quick operation.

It also stores other general informations about these "WASD" inputs.
*/
public class PIA_RunningProcessing : MonoBehaviour
{
    public Vector3 SpaceWishDir;
    public Vector2 WishDir;
    public Vector2 RunningAxis;
    public float LastBackward = Mathf.NegativeInfinity;

    private PI_AMapsManager inputsMM;

    [SerializeField] Transform playerDir;
    [SerializeField] Transform cameraDir;

    void FixedUpdate()
    {
        RunningAxis = inputsMM.playerInputActions.Arena.Running.ReadValue<Vector2>();
        if(RunningAxis.y < 0)
        {
            LastBackward = Time.time;
        }
        WishDir = playerDir.forward * RunningAxis.y + playerDir.right * RunningAxis.x;
        SpaceWishDir = cameraDir.forward * RunningAxis.y + cameraDir.right * RunningAxis.x;
    }
}
