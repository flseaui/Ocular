using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    private bool _state;

    [SerializeField]
    private GameObject _model;

    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = _model.transform.localPosition;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _state = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _state = false;
        }
    }

    private void Update()
    {
        if (_state)
        {
            _model.transform.localPosition = _initialPosition + Vector3.down;
        }
        else
        {
            _model.transform.localPosition = _initialPosition;
        }
    }
}
