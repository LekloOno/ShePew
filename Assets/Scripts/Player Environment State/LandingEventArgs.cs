using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LandingEventArgs : EventArgs
{
    private float speed;
    public LandingEventArgs(float speed){
        this.speed = speed;
    }

    public float Speed {get=>speed;}
}
