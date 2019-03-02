using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Debug.Log("Goal!");
        }
    }
}
