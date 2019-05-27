using Game;
using Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
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
    }
}