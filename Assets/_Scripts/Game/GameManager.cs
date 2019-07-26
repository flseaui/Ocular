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
        [CanBeNull] public GameObject Player;
        [CanBeNull] private GameObject _indicator;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _playerPrefab;

        public static Action OnLevelLoad;

        /// <summary>
        /// Stops the current game, unloading the level and destroying entities.
        /// </summary>
        public void StopPlaying()
        {
            Destroy(Player.gameObject);
            Destroy(_indicator.gameObject);
            _levelController.UnloadLevel();
        }
        
        /// <summary>
        /// Loads a specific level.
        /// </summary>
        /// <param name="level">
        /// The level to be loaded.
        /// </param>
        public void PlayLevel(GameObject level)
        {
            _levelController.LoadLevel(level);
            //_glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
            _indicator = Instantiate(_indicatorPrefab);
            Player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position ,
                Quaternion.identity);
        }
        
        private void Awake()
        {
            _levelController = GetComponent<LevelController>();
            _glassesController = GetComponent<GlassesController>();

            OnLevelLoad += () =>
            {
                _levelController.LoadNextLevel();
                //_glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
                Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                    Quaternion.identity);
            };
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("PlayFromEditor") == 0)
            {
                _levelController.LoadLevel();
                //_glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
                Instantiate(_indicatorPrefab);
                Player = Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                    Quaternion.identity);
            }
        }        
    }
}