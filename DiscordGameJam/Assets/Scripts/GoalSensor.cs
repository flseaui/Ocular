using UnityEngine;

public class GoalSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Goal":
                Debug.Log("GOAL!");
                break;
        }
    }
}
