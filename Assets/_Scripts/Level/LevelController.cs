using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level {
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levels;

        private GameObject _loadedLevel;
        private int _loadedLevelNumber;

        [HideInInspector]
        public LevelInfo CurrentLevelInfo;

        [ValueDropdown("_levels")] public GameObject StartingLevel;

        private int LevelCount => _levels.Count;

        public static Action OnLevelLoaded;
        
        public void LoadNextLevel()
        {
            LoadLevel(_loadedLevelNumber + 1);
        }

        public void LoadLevel()
        {
            LoadLevel(_levels.IndexOf(StartingLevel));
        }

        public void LoadLevel(GameObject level)
        {
            if (_loadedLevel != null)
                UnloadLevel();
            _loadedLevel = Instantiate(level);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();
            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            OnLevelLoaded?.Invoke();
        }
        
        public void LoadLevel(int levelNumber)
        {
            if (_loadedLevel != null)
                UnloadLevel();
            _loadedLevel = Instantiate(_levels[levelNumber]);
            _loadedLevel.gameObject.SetActive(true);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();
            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            _loadedLevelNumber = levelNumber;
            OnLevelLoaded?.Invoke();
        }

        public void UnloadLevel()
        {
            Destroy(_loadedLevel);
        }
    }
}