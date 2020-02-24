using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Level.Objects.Clones;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Level
{
    public class EntityManager : MonoBehaviour
    {
        [CanBeNull] [NonSerialized] public GameObject Player;

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _clonePrefab;

        private LevelController _levelController;

        public static Action OnEntitiesSpawned;

        private List<GameObject> _entities;
        
        private void Awake()
        {
            _entities = new List<GameObject>();
            
            Addressables.LoadAssetAsync<GameObject>("clone_prefab").Completed += handle =>
                {
                    _clonePrefab = handle.Result;
                };

            if (PlayerPrefs.GetInt("PlayFromEditor") != 1)
            {
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            }
        }

        public void ClearEntities()
        {
            foreach (var entity in _entities)
            {
                Destroy(entity);
            }
            _entities.Clear();
        }

        [UsedImplicitly]
        public void SpawnPlayer()
        {
            LevelController.Falling = false;

            if (_levelController == null)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            
            Player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position + new Vector3(0, .1f, 0),
                Quaternion.identity);

            foreach (var cloneSpawn in _levelController.CurrentLevelInfo.gameObject.transform.Find("MainFloor")
                .GetComponentsInChildren<CloneSpawn>())
            {
                _entities.Add(Instantiate(_clonePrefab, cloneSpawn.transform.position + new Vector3(0, .65f, 0), Quaternion.identity));
            }
            
            OnEntitiesSpawned?.Invoke();
            LevelController.LevelTransitioning = false;
        }
    }
}