using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/*
This script Processes the Arena Action map running inputs with the flatDir into a WishDir,
so that each "WASD" horizontal movements scripts don't have to redo this quick operation.

It also stores other general informations about these "WASD" inputs.
*/
public class PIA_RunningProcessing : MonoBehaviour
{
    public EventHandler OnStopOrLess;
    public EventHandler<Vector3> KeyPressed;
    public Vector3 SpaceWishDir;
    public Vector3 WishDir;
    public Vector2 RunningAxis;
    public Vector2 nextRunningAxis;
    public float LastBackward = Mathf.NegativeInfinity;

    [SerializeField] private PI_AMapsManager inputsMM;

    [SerializeField] Transform flatDir;
    [SerializeField] Transform sightPosition;

    void Start()
    {
        inputsMM.playerInputActions.Arena.Forward.performed += OnKeyPressed;
        inputsMM.playerInputActions.Arena.Backward.performed += OnKeyPressed;
        inputsMM.playerInputActions.Arena.Right.performed += OnKeyPressed;
        inputsMM.playerInputActions.Arena.Left.performed += OnKeyPressed;
    }

    public void OnKeyPressed(InputAction.CallbackContext obj)
    {
        nextRunningAxis = inputsMM.playerInputActions.Arena.Running.ReadValue<Vector2>();
        KeyPressed?.Invoke(this, flatDir.forward * nextRunningAxis.y + flatDir.right * nextRunningAxis.x);
    }

    void FixedUpdate()
    {
        nextRunningAxis = inputsMM.playerInputActions.Arena.Running.ReadValue<Vector2>();
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
