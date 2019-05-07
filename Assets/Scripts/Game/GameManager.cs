using System;
using System.Collections;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Level;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour
    {
        private GlassesController _glassesController;
        private LevelController _levelController;
        [CanBeNull] private GameObject _player;
        [CanBeNull] private GameObject _indicator;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _playerPrefab;

        public static Action OnLevelLoad;

        public void StopPlaying()
        {
            Destroy(_player.gameObject);
            Destroy(_indicator.gameObject);
            _levelController.UnloadLevel();
        }
        
        public void PlayLevel(GameObject level)
        {
            _levelController.LoadLevel(level);
            _glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
            _indicator = Instantiate(_indicatorPrefab);
            _player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                Quaternion.identity);
        }
        
        private void Awake()
        {
            _levelController = GetComponent<LevelController>();
            _glassesController = GetComponent<GlassesController>();

            OnLevelLoad += () =>
            {
                _levelController.LoadNextLevel();
                _glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
                Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                    Quaternion.identity);
            };
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("PlayFromEditor") == 0)
            {
                _levelController.LoadLevel();
                _glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
                Instantiate(_indicatorPrefab);
                Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                    Quaternion.identity);
            }
        }        
    }
}