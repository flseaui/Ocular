using System;
using JetBrains.Annotations;
using Level;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour
    {
        private LevelController _levelController;
        
        [CanBeNull] private GameObject _indicator;
        [SerializeField] private GameObject _indicatorPrefab;

        public static Action OnLevelLoad;

        private void Awake()
        {
            _levelController = GetComponent<LevelController>();

            OnLevelLoad += OnLoad;
        }

        private void Start()
        {
            // If NOT playing from editor
            if (PlayerPrefs.GetInt("PlayFromEditor") == 0)
            {
                _levelController.LoadFirstLevel();
                
                Instantiate(_indicatorPrefab);
            }
        } 
        
        /// <summary>
        /// Stops the current game, unloading the level and destroying entities.
        /// </summary>
        public void StopPlaying()
        {
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
            _levelController.LoadLevelFromObj(level);
            _indicator = Instantiate(_indicatorPrefab);
        }
        
        private void OnLoad()
        {
            StartCoroutine(_levelController.LoadNextLevel());
        }

        private void OnDestroy()
        {
            OnLevelLoad -= OnLoad;

        }
    }
}