﻿using System;
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
        private EntityManager _entityManager;
        
        [CanBeNull] private GameObject _indicator;
        [SerializeField] private GameObject _indicatorPrefab;

        public static Action OnLevelLoad;

        public GameObject Player => _entityManager.Player;
        
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
        
        private void Awake()
        {
            _levelController = GetComponent<LevelController>();
            _glassesController = GetComponent<GlassesController>();

            OnLevelLoad += OnLoad;

            LevelController.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnLoad()
        {
            StartCoroutine(_levelController.LoadNextLevel());
        }

        private void OnLevelLoaded()
        {
            if (PlayerPrefs.GetInt("PlayFromEditor") != 1)
                _entityManager = _levelController.CurrentLevelInfo.GetComponent<EntityManager>();
        }

        public void SetEntityManager(EntityManager manager)
        {
            _entityManager = manager;
        }
        
        private void OnDestroy()
        {
            OnLevelLoad -= OnLoad;
            LevelController.OnLevelLoaded -= OnLevelLoaded;

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
    }
}