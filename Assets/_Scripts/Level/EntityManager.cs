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
            if (PlayerPrefs.GetInt("PlayFromEditor") != 1)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
        }

        [UsedImplicitly]
        public void SpawnPlayer()
        {
            LevelController.Falling = false;

            if (_levelController == null)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            
            Player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                Quaternion.identity);
        }
    }
}