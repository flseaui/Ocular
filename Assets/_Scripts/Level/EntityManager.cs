using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Level.Objects;
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
        [SerializeField] private GameObject _directionalClonePrefab;
        
        private LevelController _levelController;
        private MapController _mapController;
        
        public static Action OnEntitiesSpawned;
        
        private List<GameObject> _entities;
        
        private void Awake()
        {
            _entities = new List<GameObject>();

            Addressables.LoadAssetAsync<GameObject>("clone_prefab").Completed += handle =>
            {
                _clonePrefab = handle.Result;
            };

            Addressables.LoadAssetAsync<GameObject>("directional_clone_prefab").Completed += handle =>
            {
                _directionalClonePrefab = handle.Result;
            };
        
            if (PlayerPrefs.GetInt("PlayFromEditor") != 1)
            {
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            }

            _mapController = GetComponent<MapController>();
        }
        
        public void ClearEntities()
        {
            foreach (var entity in _entities)
            {
                Destroy(entity.gameObject);
            }
            _entities.Clear();
        }

        public void DespawnPlayer()
        {
            Destroy(Player);
        }
        
        [UsedImplicitly]
        public void SpawnEntities()
        {
            LevelController.Falling = false;

            if (_levelController == null)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();

            // Spawn clones
            var cloneSpawnCount = _mapController.CloneSpawns.Count;
            for (var i = 0; i < cloneSpawnCount; ++i)
            {
                var spawn = _mapController.CloneSpawns[i];
                var clone = Instantiate(_clonePrefab, spawn.transform.position + new Vector3(0, .65f, 0),
                    Quaternion.identity);

                clone.GetComponent<Colorable>().OcularColor = spawn.GetComponent<Colorable>().OcularColor;
                _entities.Add(clone);
            }
            
            // Spawn directional clones
            var directionalCloneSpawnCount = _mapController.DirectionalCloneSpawns.Count;
            for (var i = 0; i < directionalCloneSpawnCount; ++i)
            {
                var spawn = _mapController.DirectionalCloneSpawns[i];
                var clone = Instantiate(_directionalClonePrefab, spawn.transform.position + new Vector3(0, .65f, 0),
                    Quaternion.identity);

                clone.GetComponent<Colorable>().OcularColor = spawn.GetComponent<Colorable>().OcularColor;
                _entities.Add(clone);
            }
   
            Player = Instantiate(_playerPrefab, _levelController.LevelInfo.PlayerSpawnPoint.transform.position + new Vector3(0, .65f, 0),
                Quaternion.identity);

            OnEntitiesSpawned?.Invoke();
            LevelController.LevelTransitioning = false;
        }
    }
}