using UnityEngine;

namespace Player {
    public class Player : MonoBehaviour
    {
        private Vector3 _spawnPos;

        private void Start()
        {
            _spawnPos = transform.position;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Death
            if (other.CompareTag("Harmful"))
            {
                transform.position = _spawnPos;
            }
        }
    }
}