using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Level;
using Level.Objects;
using Misc;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

namespace OldEditor
{
    public class LevelEditor : MonoBehaviour
    {
        public GameObject[,,] _currentLevel;

        private ColorPalette _colorPalette;
        private ObjectDrawer _objectDrawer;
        private GameObject _currentObject;
        private GameObject _level;

        /*
         * 0 - 5x5
         * 1 - 6x6
         * 2 - 7x7
         * 3 - 8x8
         * 4 - 9x9
         */
        [SerializeField] private GameObject[] _levelBasePrefabs;

        [SerializeField] GameManager _gameManager;
        [SerializeField] GameObject _glassesContainer;

        [ShowInInspector]
        private List<GameObject> _limitedObjects;
        private static readonly int OnScreen = Animator.StringToHash("OnScreen");

        public static Action<bool> OnLevelPlayToggle;

        private readonly Vector3Int _levelDimensions = new Vector3Int(100, 100, 100);

        private void Awake()
        {
            _limitedObjects = new List<GameObject>();
            _currentLevel = new GameObject[_levelDimensions.x, _levelDimensions.y, _levelDimensions.z];
            _colorPalette = GameObject.Find("ColorPalette").GetComponent<ColorPalette>();
            _objectDrawer = GameObject.Find("ObjectDrawer").GetComponent<ObjectDrawer>();
            ObjectDrawer.OnObjectSelectionChanged += @object => { _currentObject = @object; };

        }

        private void Start()
        {
            var level = PlayerPrefs.GetString("LevelToLoad");
            var size = PlayerPrefs.GetInt("LevelSize");
            if (level == "New")
            {
                NewLevel(size);
            }
            else
            {
                var levelGameObject = (GameObject)AssetDatabase.LoadAssetAtPath(level, typeof(GameObject));
                LoadLevel(levelGameObject);
            }
        }

        public void LoadLevel(GameObject level)
        {
            _level = Instantiate(level).transform.Find("MainFloor").gameObject;
            GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            _level.transform.ForEachChild(x =>
            {
                if (x.HasComponent<MaxCount>())
                    _limitedObjects.Add(x.gameObject);
            });
            Camera.main.GetComponent<EditorCameraCenter>().LevelInfo =
                _level.transform.parent.GetComponent<LevelInfo>();
        }

        public void NewLevel(int size)
        {
            _level = Instantiate(_levelBasePrefabs[size]).transform.Find("MainFloor").gameObject;
            _level.transform.parent.GetComponent<LevelInfo>().Name = "BlankLevel";
            GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            _level.transform.ForEachChild(x =>
            {
                if (x.HasComponent<MaxCount>())
                    _limitedObjects.Add(x.gameObject);
            });
        }

        public void TestLevel()
        {
            OnLevelPlayToggle?.Invoke(!_glassesContainer.activeSelf);
            //TODO Refactor the rest of these into their own scripts invoked by OnLevelPlayToggle
            _colorPalette.GetComponent<Animator>().SetBool(OnScreen, !_colorPalette.GetComponent<Animator>().GetBool(OnScreen));
            _objectDrawer.GetComponent<Animator>().SetBool(OnScreen, !_objectDrawer.GetComponent<Animator>().GetBool(OnScreen));
            PlayerPrefs.SetInt("PlayFromEditor", 1);
            _gameManager.gameObject.SetActive(!_gameManager.gameObject.activeSelf);
            _glassesContainer.SetActive(!_glassesContainer.activeSelf);
            if (!_glassesContainer.activeSelf)
                _gameManager.StopPlaying();
            else
                _gameManager.PlayLevel(_level.transform.parent.gameObject);
            _level.transform.parent.gameObject.SetActive(!_level.transform.parent.gameObject.activeSelf);

        }

        public void SaveLevel()
        {
            if (_level.transform.parent.GetComponent<LevelInfo>().Name == "BlankLevel")
            {
                var levelName =
                    $"Assets/_Prefabs/Levels/Level{_level.GetHashCode()}.prefab";
                _level.transform.parent.GetComponent<LevelInfo>().Name = levelName;
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, levelName);
            }
            else
            {
                var level = PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, _level.transform.parent.GetComponent<LevelInfo>().Name);
            }
            HiResScreenshot.TakeHiResShot($"Level{_level.GetHashCode()}");
            SceneManager.LoadSceneAsync("EditorMenu");
        }

        public void PlaceElement(Vector3Int position, Orientation orientation, Direction direction)
        {
            var element = (GameObject)PrefabUtility.InstantiatePrefab(_currentObject, _level.transform);
            element.transform.position = position;

            if (element.transform.HasComponent<SlopeWalkable>(out var slope))
                slope.MatchRotation(orientation, direction);
            else if (element.CompareTag("PlayerSpawn"))
                _level.GetComponentInParent<LevelInfo>().PlayerSpawnPoint = element.transform.Find("Model").transform;

            if (element.transform.HasComponent<MaxCount>(out var max))
            {
                var theseObjects = _limitedObjects.Where(x => x.name == element.name || x.name.Contains(element.name)).ToArray();

                if (theseObjects.Length >= max.Max)
                {
                    var obj = theseObjects.First();
                    _limitedObjects.Remove(obj);
                    Destroy(obj);
                }

                _limitedObjects.Add(element);
            }

            if (element.transform.HasComponent<Colorable>(out var colorable))
                colorable.OcularState = _colorPalette.SelectedColor;

            var pos = new Vector3Int
            (
                _levelDimensions.x / 2 + position.x,
                _levelDimensions.y / 2 + position.y,
                _levelDimensions.z / 2 + position.z
            );

            _currentLevel[pos.x, pos.y, pos.z] = element;
        }
    }
}

#endif
