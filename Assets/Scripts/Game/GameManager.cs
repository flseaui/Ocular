using System;
using Level;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour
    {
        private GlassesController _glassesController;
        [SerializeField] private GameObject _indicator;
        private LevelController _levelController;

        [SerializeField] private GameObject _playerPrefab;

        public static Action OnLevelLoad;
        
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
            _levelController.LoadLevel();
            _glassesController.ResetGlasses(_levelController.CurrentLevelInfo.LevelGlasses);
            Instantiate(_indicator);
            Instantiate(_playerPrefab, _levelController.CurrentLevelInfo.PlayerSpawnPoint.transform.position,
                Quaternion.identity);
        }        
    }
}