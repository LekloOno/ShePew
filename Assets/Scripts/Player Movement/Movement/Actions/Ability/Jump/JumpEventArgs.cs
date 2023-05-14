using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JumpEventArgs : EventArgs
{
    private float force;
    public JumpEventArgs(float force)
    {
        this.force = force;
    }

    public float Force {get => force;}
}
