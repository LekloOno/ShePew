using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script initiate the player's input actions, and enable and disable the right action maps during the game.
*/

public class PI_AMapsManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    //For now, we only care about arena, so we enable arena On this Enable, but this will later have the job to switch between action maps.
    void OnEnable()
    {
        playerInputActions.Arena.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Arena.Disable();
    }
}
