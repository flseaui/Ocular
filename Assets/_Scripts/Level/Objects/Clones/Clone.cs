using System;
using Game;
using Misc;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Objects
{
    public class Clone : MonoBehaviour
    {
        private GameObject _player;
        
        public bool Falling;
        public bool Died;

        public void ActuallyDie()
        {
            
        }
        
        private void Awake()
        {
            EntityManager.OnEntitiesSpawned += OnEntitiesSpawned;
        }

        private void OnDestroy()
        {
            EntityManager.OnEntitiesSpawned -= OnEntitiesSpawned;
        }

        private void OnEntitiesSpawned()
        {
            Debug.Log(GameObject.Find("GameManager").GetComponent<GameManager>().Player);
            _player = GameObject.Find("GameManager").GetComponent<GameManager>().Player;
            //GetComponent<ClonePathfinder>().WalkSpeed = _player.GetComponent<Pathfinder>().WalkSpeed;
        }

        private void Update()
        {
            if (Pathfinder.Navigating && !GetComponent<ClonePathfinder>().Navigating)
            {
                GetComponent<ClonePathfinder>().MirrorClone(_player.GetComponent<Pathfinder>().GetCurrentWalkable(out _), _player.GetComponent<Pathfinder>()._currentEnd);
            }
        }
    }
}