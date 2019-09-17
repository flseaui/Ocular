using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Level
{
    public class EntityManager : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public GameObject Player;
        
        [SerializeField] private GameObject _playerPrefab;

        private LevelController _levelController;
        
        private void Awake()
        {
            _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
        }
        
        [UsedImplicitly]
        public void SpawnPlayer()
        {
            Player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                Quaternion.identity);
        }    
    }
}