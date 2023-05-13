using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PMA_Ability<T> : PM_BaseAction<T> where T : DATA_BaseAction
{
    public event EventHandler OnFixedUpdate;
    protected InputAction action;
    [SerializeField] protected PI_AMapsManager inputMapsManager;

    protected virtual void FixedUpdate()
    {
        OnFixedUpdate?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void Start()
    {
        action.performed += StartAbility;
        action.canceled += StopAbility;
    }

    public virtual void StartAbility(InputAction.CallbackContext obj){}

    public virtual void StopAbility(InputAction.CallbackContext obj){}
}
