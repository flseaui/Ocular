using Game;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{

    public static class Extensions
    {
        public static (int, int) CoordinatesOf<T>(this T[,] matrix, T value)
        {
            var w = matrix.GetLength(0);
            var h = matrix.GetLength(1);

            for (var x = 0; x < w; ++x)
            {
                for (var y = 0; y < h; ++y)
                {
                    if (matrix[x, y] == null) continue;

                    if (matrix[x, y].Equals(value))
                        return (x, y);
                }
            }
            Debug.Log("Item not found");
            return (-1, -1);
        }

        public static (int, int) GetNext<T>(this T[,] matrix, (int, int) value)
        {
            var w = matrix.GetLength(0);
            var h = matrix.GetLength(1);

            for (var x = value.Item1; x < w; ++x)
            {
                for (var y = x == value.Item1 ? value.Item2 + 1 : 0; y < h; ++y)
                {
                    if (!ReferenceEquals(matrix[x, y], null))
                        return (x, y);
                }
            }
            Debug.Log("Item is last item in matrix");
            return (-1, -1);
        }
    }

    public class LevelController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _worldOneLevels;
        [SerializeField] private List<GameObject> _worldTwoLevels;
        [SerializeField] private List<GameObject> _worldThreeLevels;
        [SerializeField] private List<GameObject> _testLevels;

        private List<List<GameObject>> _worlds;

        private GameObject[,] _levels = new GameObject[4, 10];

        private GameObject _loadedLevel;
        private (int, int) _loadedLevelNumber;

        [ShowInInspector, ReadOnly]
        public LevelInfo CurrentLevelInfo;

        public List<GameObject> AllWorldLevels => _worldOneLevels.Concat(_worldTwoLevels).Concat(_worldThreeLevels).Concat(_testLevels).ToList();

        [ValueDropdown("AllWorldLevels"), ListDrawerSettings(NumberOfItemsPerPage = 10), SerializeField]
        public GameObject StartingLevel;

        public static bool Falling;

        public static Action OnLevelLoaded;

        private (int, int) _levelToLoad;

        public static (int, int) StartingLevelIndex = (-1, -1);

        private void Awake()
        {
            _worlds = new List<List<GameObject>>
            {
                _worldOneLevels,
                _worldTwoLevels,
                _worldThreeLevels,
                _testLevels
            };

            for (var i = 0; i < _worlds.Count; i++)
            {
                var world = _worlds[i];
                for (var j = 0; j < world.Count; j++)
                {
                    _levels[i, j] = world[j];
                }
            }
        }

        public IEnumerator LoadNextLevel()
        {
            _levelToLoad = _levels.GetNext(_loadedLevelNumber);
            CurrentLevelInfo.Animator.SetTrigger("FadeOut");

            yield return new WaitUntil(() => CurrentLevelInfo.ReadyToLoad);

            CurrentLevelInfo.ReadyToLoad = false;
            LoadLevel();

            GameObject.Find("GameManager").GetComponent<GlassesController>().UpdateOcularState();
        }

        public void LoadFirstLevel()
        {
            if (StartingLevelIndex != (-1, -1))
                _levelToLoad = StartingLevelIndex;
            else
                _levelToLoad = _levels.CoordinatesOf(StartingLevel);

            LoadLevel();
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

        public void RestartLevel()
        {
            LoadLevel();
            GetComponent<GlassesController>().UpdateOcularState();
        }

        private void LoadLevel()
        {
            if (_loadedLevel != null)
                UnloadLevel();

            _loadedLevel = Instantiate(_levels[_levelToLoad.Item1, _levelToLoad.Item2]);
            _loadedLevel.gameObject.SetActive(true);
            _loadedLevel.GetComponent<MapController>().FindNeighbors();

            CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
            _loadedLevelNumber = _levelToLoad;
            OnLevelLoaded?.Invoke();
            CurrentLevelInfo.Animator.SetTrigger("FadeIn");

            GetComponent<GlassesController>().CheckForNewWorldMusic();
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
