using Sirenix.OdinInspector;
using UnityEngine;

public class Button : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    public bool State;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            State = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            State = false;
        }
    }
}