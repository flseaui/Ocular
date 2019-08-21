using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Level {
    
    public static class Extensions
    {
        public static Tuple<int, int> CoordinatesOf<T>(this T[,] matrix, T value)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }
            Debug.Log("Item not found");
            return Tuple.Create(-1, -1);
        }
        
        public static Tuple<int, int> GetNext<T>(this T[,] matrix, Tuple<int, int> value)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            for (int x = value.Item1; x < w; ++x)
            {
                for (int y = value.Item2 + 1; y < h; ++y)
                {
                    if (!ReferenceEquals(matrix[x, y], null))
                        return Tuple.Create(x, y);
                }
            }
            Debug.Log("Item is last item in matrix");
            return Tuple.Create(-1, -1);
        }
    }

    public class LevelController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _worldOneLevels;
        [SerializeField] private List<GameObject> _worldTwoLevels;

        private List<List<GameObject>> _worlds;

        private GameObject[,] _levels = new GameObject[2, 6];

        private GameObject _loadedLevel;
        private Tuple<int, int> _loadedLevelNumber;

        [HideInInspector]
        public LevelInfo CurrentLevelInfo;

        [ValueDropdown("_worldOneLevels")] public GameObject StartingLevel;

        public static Action OnLevelLoaded;

        private Tuple<int, int> _levelToLoad;
        
        private void Awake()
        {      
            _worlds = new List<List<GameObject>>
            {
                _worldOneLevels,
                _worldTwoLevels
            };
            
            var worldIndex = 0;
            foreach (var world in _worlds)
            {
                var levelIndex = 0;
                foreach (var level in world)
                {
                    _levels[worldIndex, levelIndex] = level;
                    levelIndex++;
                }

                worldIndex++;
            }
        }

        public IEnumerator LoadNextLevel()
        {
            _levelToLoad = _levels.GetNext(_loadedLevelNumber);
            CurrentLevelInfo.Animator.SetTrigger("FadeOut");
            
            yield return new WaitUntil(() => CurrentLevelInfo.ReadyToLoad);
            CurrentLevelInfo.ReadyToLoad = false;
            StartCoroutine(nameof(LoadLevel));
        }

        public IEnumerator LoadFirstLevel()
        {
            _levelToLoad = _levels.CoordinatesOf(StartingLevel);
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
            
            _loadedLevel = Instantiate(_levels[_levelToLoad.Item1, _levelToLoad.Item2]);
            _loadedLevel.gameObject.SetActive(true);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();
            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            _loadedLevelNumber = _levelToLoad;
            OnLevelLoaded?.Invoke();
            CurrentLevelInfo.Animator.SetTrigger("FadeIn");
        }

        public void UnloadLevel()
        {
            Destroy(_loadedLevel);
            CurrentLevelInfo = null;
        }

        public int GetCurrentWorld()
        {
            return _loadedLevelNumber.Item1;
        }

    }
}