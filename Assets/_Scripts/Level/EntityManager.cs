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
                Debug.Log("whoa whoa whoa who said we could have this much epic around");
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            }
        }
        
        public void ClearEntities()
        {
            Debug.Log($"cleawin {_entities.Count} entities!");
            foreach (var entity in _entities)
            {
                Debug.Log(entity.name + " is getting pwnd");
                Destroy(entity.gameObject);
            }
            _entities.Clear();
        }

        public void DespawnPlayer()
        {
            Destroy(Player);
        }
        
        public void SpawnClone(Colorable spawn)
        {
            var clone = Instantiate(_clonePrefab, spawn.transform.position + new Vector3(0, .65f, 0),
                Quaternion.identity);

            clone.GetComponent<Colorable>().QueueOcularStateChange(spawn.OcularState);
            _entities.Add(clone);
        }
        
        [UsedImplicitly]
        public void SpawnEntities()
        {
            LevelController.Falling = false;

            if (_levelController == null)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            
            Player = Instantiate(_playerPrefab, _levelController.LevelInfo.PlayerSpawnPoint.transform.position + new Vector3(0, .65f, 0),
                Quaternion.identity);

            OnEntitiesSpawned?.Invoke();
            LevelController.LevelTransitioning = false;
        }
    }
}