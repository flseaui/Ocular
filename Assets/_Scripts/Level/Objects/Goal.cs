using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Objects {
    public class Goal : MonoBehaviour
    {
        [ShowInInspector]
        public static int GoalConditions;

        [ShowInInspector] public static int GoalConditionsMet;

        public bool Satisfied;

        private Animator _animator;
        private static readonly int SatisfiedProp = Animator.StringToHash("satisfied");

        private void Awake()
        {
            _animator = transform.parent.GetComponent<Animator>();
            Satisfied = true;
        }

        private void Update()
        {
            Satisfied = GoalConditionsMet >= GoalConditions;
            _animator.SetBool(SatisfiedProp, Satisfied);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!Satisfied) return;
            
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
