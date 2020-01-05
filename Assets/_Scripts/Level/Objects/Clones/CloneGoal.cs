using UnityEngine;

namespace Level.Objects.Clones
{
    public class CloneGoal : MonoBehaviour
    {
        private void Awake()
        {
            Goal.GoalConditions += 1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Clone"))
            {
                Goal.GoalConditionsMet += 1;
                other.transform.GetComponent<ClonePathfinder>().AtGoal = true;
                other.transform.GetComponent<ClonePathfinder>().Navigating = false;
            }
        }
    }
}