using System;
using OcularAnimation.New;
using UnityEngine;

namespace Level.Objects.Clones
{
    public class CloneGoal : MonoBehaviour
    {
        private bool _used;
        
        private void Awake()
        {
            Goal.GoalConditions += 1;
            Player.Player.OnDeath += OnPlayerDeath;
        }

        private void OnDestroy()
        {
            Player.Player.OnDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            _used = false;
            Debug.Log("oh baby we are in there now");
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Clone") && !_used)
            {
                _used = true;
                Goal.GoalConditionsMet += 1;
                other.transform.GetComponent<ClonePathfinder>().AtGoal = true;
                other.transform.GetComponent<ClonePathfinder>().Navigating = false;
                other.transform.GetComponent<CloneAnimationController>().SetCurrentGoal(this);
            }
        }
    }
}