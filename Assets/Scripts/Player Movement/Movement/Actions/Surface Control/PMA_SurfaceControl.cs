using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_SurfaceControl<T> : PM_BaseAction<T> where T : DATA_SurfaceControl
{

    public event EventHandler OnFixedUpdate;
    [SerializeField] protected PM_SC_Manager surfaceControlManager;
    
    protected virtual void Awake()
    {
        _groundState.OnLanding += SurfaceControl_OnLandingSurface;
        _groundState.OnLeavingGround += SurfaceControl_OnLeavingSurface;
    }/*
    protected virtual void OnDisable()
    {
        _groundState.OnLanding -= SurfaceControl_OnLandingSurface;
        _groundState.OnLeavingGround -= SurfaceControl_OnLeavingSurface;
    }*/

    public virtual void ActivateData(){
        surfaceControlManager.CurrentData = data;
    }

    public virtual void SurfaceControl_OnLandingSurface(object sender, EventArgs e){}

    public virtual void SurfaceControl_OnLeavingSurface(object sender, EventArgs e){}

    protected void FixedUpdate()
    {
        OnFixedUpdate?.Invoke(this, EventArgs.Empty);
    }
}