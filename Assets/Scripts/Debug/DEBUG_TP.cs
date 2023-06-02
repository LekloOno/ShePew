using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DEBUG_TP : MonoBehaviour
{
    [SerializeField] private PI_AMapsManager _inputMapsManager;
    [SerializeField] private GameObject _pivot;
    [SerializeField] private Transform _tpPoint;

    void Start()
    {
        _inputMapsManager.playerInputActions.Arena.DEBUG_TP.performed += DEBUG_TP_response;
    }

    public void DEBUG_TP_response(InputAction.CallbackContext obj)
    {
        _pivot.transform.position = _tpPoint.position;
    }
}
