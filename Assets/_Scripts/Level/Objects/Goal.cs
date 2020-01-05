using Player;
using UnityEngine;

namespace Level.Objects {
    public class Goal : MonoBehaviour
    {
        public static int GoalConditions;
        public static int GoalConditionsMet;
        
        private void OnTriggerEnter(Collider other)
        {
            if (GoalConditionsMet < GoalConditions) return;
            
            if (other.CompareTag("Player"))
            {
                GoalConditions = 0;
                GoalConditionsMet = 0;
                Pathfinder.Navigating = false;
                Pathfinder.AtGoal = true;
                //ES3.Save<int>("LatestLevel", GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo.Name);
            }
        }
    }
}
