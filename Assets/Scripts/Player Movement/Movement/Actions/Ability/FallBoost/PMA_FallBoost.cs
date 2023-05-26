using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_FallBoost : MonoBehaviour
{
    //https://www.desmos.com/calculator/gwu1lcucky?lang=fr
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PM_SC_Manager _surfaceControlManager;
    [SerializeField] private float _minSpeed = 4f;
    [SerializeField] private float _boostStrength = 0.04f;
    [SerializeField] private float _boostCurveSmoothness = 0.4f;
    [SerializeField] private float _boostTime = 2f;
    [SerializeField] private float _decayStrength = 1.5f;
    private float _initBoost;
    private static string _fallBoostModifierKey = "FallBoost";
    private float _startTime;

    private event EventHandler OnFixedUpdate;
    
    void Start()
    {
        _groundState.OnLandingInfos += OnLandingInfos_FallBoost;
        _groundState.OnLeavingGround += OnLeavingGround_FallBoost; 
    }

    void FixedUpdate()
    {
        OnFixedUpdate?.Invoke(this, EventArgs.Empty);
    }

    public void OnLandingInfos_FallBoost(object sender, LandingEventArgs e)
    {
        Debug.Log(e.Speed);
        if(e.Speed >= _minSpeed)
        {
            _initBoost = 1+Mathf.Pow(_boostStrength*(e.Speed-_minSpeed), _boostCurveSmoothness);
            _startTime = Time.time;
            _surfaceControlManager.MaxSpeedModifiers[_fallBoostModifierKey] = _initBoost;
            OnFixedUpdate += OnFixedUpdate_FallBoost;
        }
    }

    public void OnLeavingGround_FallBoost(object sender, EventArgs e)
    {
        StopBoost();
    }

    public void OnFixedUpdate_FallBoost(object sender, EventArgs e)
    {
        float timeStep = (Time.time-_startTime)/_boostTime;
        if(timeStep > 1)
        {
            StopBoost();
        }
        else
        {
            _surfaceControlManager.MaxSpeedModifiers[_fallBoostModifierKey] = Mathf.Lerp(_initBoost, 1, Mathf.Pow(timeStep, _decayStrength));
        }
    }

    public void StopBoost()
    {
        _surfaceControlManager.MaxSpeedModifiers.Remove(_fallBoostModifierKey);
        OnFixedUpdate -= OnFixedUpdate_FallBoost;
    }
}
