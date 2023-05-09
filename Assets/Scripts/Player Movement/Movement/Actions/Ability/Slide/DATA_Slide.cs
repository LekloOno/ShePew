using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DATA_Slide", menuName = "Player Movement/Actions/Slide Data")]
public class DATA_Slide : DATA_BaseAction
{
    [Header("Slide Specifics")]
    public float SlideXForce = 5;               //The base slide boost strength. Maximum horizontal force applied when starting a slide.
    public float AirForceMultiplier = 0.4f;     //Multiplier applied to the horizontal force when the player is starting the slide in the air.

    //The slide boost has a decay. If the player start two slide in a row, it will be reduced.

    public float SlideDecayRecover = 3;
    //SlideDecayRecover is the time required after starting a slide to get the full slide boost.
    public float SlideDecayMinRecover = 1; 
    //SlideDecayMinRecover is the minimum recover of the boost.
    //It means if the player hold his slide for more than the SlideDecayRecover time, he will still have this minimum amount of time to recover the full boost.
    public float SlideDecayStrength = 0.8f;
    //This value changes the behavior of the recovering curve.
    //x == 1 Means it's linear
    //x < 1 means it's logarithmic (so the resulting force will be on averrage higher)*
    //x > 1 means it's exponential (so the resulting force will be on averrage lower)*
    //
    //*This exponent is used on the force multiplier, which will be between 0 and 1. real force = SlideXForce * (Time spent/SlideDecayRecover)^SlideDecayStrength

    public PhysicMaterial SlideMat;             //Physic Material used when sliding
}
