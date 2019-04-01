using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Button : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    public bool State;

    public Action OnStateChanged;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (State) return;
            State = true;
            OnStateChanged?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (!State) return;
            State = false;
            OnStateChanged?.Invoke();
        }
    }
}