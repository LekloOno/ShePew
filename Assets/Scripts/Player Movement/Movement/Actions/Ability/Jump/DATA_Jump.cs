using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_Jump", menuName = "Player Movement/Actions/Jump Data")]
public class DATA_Jump : DATA_BaseAction
{
    [Header("Strength")]
    public float HeldJumpForce = 3.8f;      //The force value used for a Held jump.
    public float TapJumpForce = 4;          //The force value used for a Tap jump.
    public float PreYMulltiplier = 0;       //Between 0 and 1 most likely. 0 means the Y velocity is completely reseted before applying the jump force, 1 means the jump only applies its jump force, no matter the context.
    //It also means that with the same force, a jump will always propulse the player of the same height if PreYMultiplier is set to 0.

    [Header("Holding")]
    public float HeldJumpCD = 0.3f;         //The cooldown between two jump when the jump bind is held down.
    public float HeldJumpDelay = 0.02f;     //The delay between the moment you land and the moment you jump if the jump is considered held.
    public float HeldJumpThreshold = 0.07f; //If you held the jump bind down since at least this much time, the jump will be considered held.
    public float HoldSpeedPenalty = 0.7f;   //The MaxSpeed Modifier applied whenever space bar is down. 1 means it has no effect, <1 means it is a penalty.

    [Header("Decay")]
    public float JumpDecayRecover = 0;      //The time required between two jump to get full jump strength.
    public float JumpDecayFloor = 0;        //The minimum multplier of the jump strength?
    public AnimationCurve JumpDecayCurve;   //Explicit
}
