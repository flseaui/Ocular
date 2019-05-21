using Game;
using UnityEngine;

namespace Player {
    public class Player : MonoBehaviour
    {
        private Vector3 _spawnPos;

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
        
        private void Start()
        {
            _spawnPos = transform.position;
        }

        public void Death()
        {
            transform.position = _spawnPos;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Harmful"))
                Death();
        }
    }
}