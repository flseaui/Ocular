using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        private int _levelToLoad;
        
        public IEnumerator LoadNextLevel()
        {
            _levelToLoad = _loadedLevelNumber + 1;
            StartCoroutine(nameof(LoadLevel));
            yield return new WaitForSeconds(1);
        }

        public IEnumerator LoadFirstLevel()
        {
            _levelToLoad = _levels.IndexOf(StartingLevel);
            StartCoroutine(nameof(LoadLevel));
            
            yield return new WaitForSeconds(1);
        }

        public void LoadLevelFromObj(GameObject level)
        {
            if (_loadedLevel != null)
                UnloadLevel();
            _loadedLevel = Instantiate(level);
            _loadedLevel.gameObject.SetActive(true);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();
            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            OnLevelLoaded?.Invoke();
        }
        
        public IEnumerator LoadLevel()
        {
            if (_loadedLevel != null)
                UnloadLevel();
            
            yield return new WaitForEndOfFrame();
            
            _loadedLevel = Instantiate(_levels[_levelToLoad]);
            _loadedLevel.gameObject.SetActive(true);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();
            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            _loadedLevelNumber = _levelToLoad;
            OnLevelLoaded?.Invoke();
        }

        public void UnloadLevel()
        {
            Destroy(_loadedLevel);
            CurrentLevelInfo = null;
        }
    }
}