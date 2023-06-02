using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PMA_FallBoost : MonoBehaviour
{
    //https://www.desmos.com/calculator/gwu1lcucky?lang=fr
    [SerializeField] private PMA_Slide _slide;
    [SerializeField] private PES_Grounded _groundState;
    [SerializeField] private PM_SC_Manager _surfaceControlManager;
    [SerializeField] private float _minSpeed = 4f;
    [SerializeField] private float _boostStrength = 0.04f;
    [SerializeField] private float _boostCurveSmoothness = 0.4f;
    [SerializeField] private float _boostTime = 2f;
    [SerializeField] private float _decayStrength = 1.5f;
    [SerializeField] private float _stunThreshold = 20f;
    [SerializeField] private float _stunSpeedMultiplier = 0.2f;
    [SerializeField] private float _stunDragMultiplier = 20;
    private float _initBoost;
    private float _initDrag;
    private static string _fallBoostModifierKey = "FallBoost";
    private float _startTime;
    private float _stunStartTime;

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
        //Debug.Log(e.Speed);
        if(e.Speed >= _minSpeed)
        {
            if(e.Speed < _stunThreshold)
            {
                _initBoost = 1+Mathf.Pow(_boostStrength*(e.Speed-_minSpeed), _boostCurveSmoothness);
                Debug.Log("Tamer");
            }
            else
            {
                Debug.Log("Stun");
                _stunStartTime = Time.time;
                _initBoost = _stunSpeedMultiplier;
                _initDrag = _stunDragMultiplier * (_slide.IsActive ? 4 : 1);
                Debug.Log(_slide.IsActive);
                _surfaceControlManager.DragModifiers[_fallBoostModifierKey] = _initDrag;
                OnFixedUpdate += OnFixedUpdate_FallStun;
            }
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

    public void OnFixedUpdate_FallStun(object sender, EventArgs e)
    {
        float timeStep = (Time.time-_stunStartTime)/_boostTime;
        if(timeStep > 1)
        {
            StopStun();
        }
        else
        {
            _surfaceControlManager.DragModifiers[_fallBoostModifierKey] = Mathf.Lerp(_initDrag, 1, Mathf.Pow(timeStep, _decayStrength));
        }
    }

    public void StopBoost()
    {
        _surfaceControlManager.MaxSpeedModifiers.Remove(_fallBoostModifierKey);
        OnFixedUpdate -= OnFixedUpdate_FallBoost;
    }

    public void StopStun()
    {
        _surfaceControlManager.DragModifiers.Remove(_fallBoostModifierKey);
        OnFixedUpdate -= OnFixedUpdate_FallStun;
    }
}
