using System.Linq;
using UnityEngine;

public class GoalSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch(other.transform.parent.tag)
        {
            case "Goal":
                Debug.Log("GOAL!");
                LevelManager.Instance.LoadNextLevel();
                break;
        }
    }
}
