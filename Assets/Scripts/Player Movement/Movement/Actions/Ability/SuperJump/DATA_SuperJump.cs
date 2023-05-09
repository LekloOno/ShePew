using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_SuperJump", menuName = "Player Movement/Actions/Super Jump Data")]
public class DATA_SuperJump : DATA_BaseAction
{
    public float FullChargeForce = 6;           //Full super jump boost, it will be added to the normal jump force.

    [Header("Charge")]
    public float FullChargeTime = 0.6f;         //Required time spent sliding on ground to get the full super jump boost.
    public float ChargeCurveStrength = 2;       //Tension of the curve describing the impact of the time spent on ground on the final force.
    public float ChargeAutoResetTime = 0.4f;    //If the player leaves the ground, but get back on ground in less than this time, the charge will not be reseted.

    [Header("Time Decay")]
    public float DecayRecover = 4;              //How much time must seperate two super jump to avoid a force penalty.
    public float DecayStrength = 2;             //Tension of this penalty curve.
}
