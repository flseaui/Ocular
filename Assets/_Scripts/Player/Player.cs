using Game;
using Level;
using Level.Objects;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private static Colorable _currentCollision;

        public void CheckForDeath(Colorable colorable)
        {
            if (colorable == _currentCollision)
                Death();
        }
        
        private void Awake()
        {
            GameManager.OnLevelLoad += CommitDie;
        }

        private void OnDestroy()
        {
            GameManager.OnLevelLoad -= CommitDie;
        }

        private void CommitDie()
        {
            Destroy(gameObject);
        }

        public void Death()
        {
            transform.position = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                .PlayerSpawnPoint.position;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.ParentHasComponent<Colorable>(out var colorable))
                _currentCollision = colorable;
            if (other.CompareTag("Harmful"))
                Death();
        }

        private void OnTriggerExit(Collider other)
        {
            _currentCollision = null;
        }
    }
}