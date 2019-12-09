using System;
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
        
        private void Awake()
        {
            Addressables.LoadAssetAsync<GameObject>("clone_prefab").Completed += handle =>
                {
                    _clonePrefab = handle.Result;
                };
            
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

            foreach (var cloneSpawn in _levelController.CurrentLevelInfo.gameObject.transform.Find("MainFloor")
                .GetComponentsInChildren<CloneSpawn>())
            {
                Instantiate(_clonePrefab, cloneSpawn.transform);
            }
            
            OnEntitiesSpawned?.Invoke();
        }
    }
}