using System.ComponentModel;
using Game;
using Level;
using Level.Objects;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private static Colorable _currentCollision;
        [ShowInInspector]
        public static bool Falling;
        
        public void CheckForDeath(Colorable colorable)
        {
            if (colorable == _currentCollision)
                Death();
        }
        
        private void Awake()
        {
            GameManager.OnLevelLoad += CommitDie;
        }

        private void Update()
        {
            Physics.Raycast(transform.localPosition, Vector3.down, out var hit, 1.5f, LayerMask.GetMask("Model"));
            if (hit.collider == null)
            {
                Falling = true;
                GetComponent<Rigidbody>().AddForce(Vector3.down * 5);
            }
            else
                Falling = false;
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