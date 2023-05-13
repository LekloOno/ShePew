using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SF_GameplayValues
{
    static float fcTopTension = 3.0f;
    static float fcMaxFactor = 0.12f;
    static float fcFactor = 10f;
    static float fcMinimum = 40f;

    public static float FrictionChargeAmount(float velocity, float fcTopOffset = 16, float fcMaxTension = 0.4f)
    {
    /*
    https://www.desmos.com/calculator/0mt6ixhqbk
    */
        return /*fcFactor/fcMinimum+*/fcFactor*(fcMaxFactor*Mathf.Pow(Mathf.Max(velocity-fcTopOffset,0),fcMaxTension)+Mathf.Min(Mathf.Pow((velocity/fcTopOffset),fcTopTension),1));
    }
}
