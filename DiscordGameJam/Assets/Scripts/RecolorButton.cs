using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class RecolorButton : MonoBehaviour
{
    public enum ButtonType
    {
        Toggle,
        Hold
    }

    [SerializeField]
    private GameObject _buttonModel;

    private float _initialY;
    
    public bool State;
    
    public ButtonType Type;

    [SerializeField]
    private GameObject[] TargetBlocks;

    public VisibilityController.BlockColor OffColor, OnColor;
    
    private void Start()
    {
        _initialY = _buttonModel.transform.position.y;
    }

    private void Update()
    {
        if (State)
        {
            _buttonModel.transform.position = new Vector3(_buttonModel.transform.position.x, _initialY - .1f,
                _buttonModel.transform.position.z);
        }
        else
        {
            _buttonModel.transform.position = new Vector3(_buttonModel.transform.position.x, _initialY,
                _buttonModel.transform.position.z);
        }
    }
    
    public void Press()
    {
        if (Type == ButtonType.Hold)
        {
            State = true;
            foreach (var block in TargetBlocks)
            {
                block.GetComponent<VisibilityController>().SetColor(OnColor);
            }
        }
        else
        {
            State = !State;
        }
    }

    public void Release()
    {
        if (Type == ButtonType.Hold)
        {
            State = false;
            foreach (var block in TargetBlocks)
            {
                block.GetComponent<VisibilityController>().SetColor(OffColor);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") || other.transform.CompareTag("Box"))
        {
            Press();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") || other.transform.CompareTag("Box"))
        {
            Release();
        }
    }
}
