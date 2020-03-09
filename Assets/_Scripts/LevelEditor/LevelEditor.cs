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

namespace LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
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

        [SerializeField] private GameObject _colorWheel;
        
        [ShowInInspector]
        private List<GameObject> _limitedObjects;
        private static readonly int OnScreen = Animator.StringToHash("OnScreen");

        public static Action<bool> OnLevelPlayToggle;

        public LevelInfo EditorLevelInfo;
        
        private void Awake()
        {
            _limitedObjects = new List<GameObject>();
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var model = _level.transform.parent.Find("Level").gameObject;
                if (model != null)
                    model.SetActive(!model.activeSelf);
            }
        }

        public void LoadLevel(GameObject level)
        {
            _level = ((GameObject) PrefabUtility.InstantiatePrefab(level)).transform.Find("MainFloor").gameObject;
            _level.transform.parent.GetComponent<Animator>().enabled = false;
            EditorLevelInfo = _level.GetComponentInParent<LevelInfo>();
            _gameManager.GetComponent<LevelController>().CurrentLevelInfo = EditorLevelInfo;
            GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            _level.transform.ForEachChild(x =>
            {
                if (x.HasComponent<MaxCount>())
                    _limitedObjects.Add(x.gameObject);
            });
            Camera.main.GetComponent<EditorCameraCenter>().LevelInfo = EditorLevelInfo;
        }

        public void NewLevel(int size)
        {
            _level = Instantiate(_levelBasePrefabs[size]).transform.Find("MainFloor").gameObject;
            _level.transform.parent.GetComponent<Animator>().enabled = false;
            _level.transform.parent.GetComponent<LevelInfo>().Name = "BlankLevel";
            _gameManager.GetComponent<LevelController>().CurrentLevelInfo = EditorLevelInfo;
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
            _gameManager.SetEntityManager(_level.transform.parent.GetComponent<EntityManager>());
            _glassesContainer.SetActive(!_glassesContainer.activeSelf);
            _colorWheel.SetActive(!_colorWheel.activeSelf);
            if (!_glassesContainer.activeSelf)
                _gameManager.StopPlaying();
            else
                _gameManager.PlayLevel(_level.transform.parent.gameObject);
            UpdateEntities();
            _level.transform.parent.gameObject.SetActive(!_level.transform.parent.gameObject.activeSelf);

        }

        public void UpdateEntities()
        {
            if (_glassesContainer.activeSelf)
            {
                _level.transform.parent.GetComponent<EntityManager>().SpawnPlayer();
            }
            else
            {
                Destroy(_level.transform.parent.GetComponent<EntityManager>().Player);
                _level.transform.parent.GetComponent<EntityManager>().ClearEntities();
            }

        }
        
        public void RemoveObject(GameObject obj)
        {
            obj.SetActive(false);
        }
        
        public void SaveLevel()
        {
            _level.transform.parent.GetComponent<Animator>().enabled = true;
            var levelGO = _level.transform.parent.Find("Level");
            if (levelGO != null)
            {
                levelGO.gameObject.SetActive(true);
            } 
            if (EditorLevelInfo.Name == "BlankLevel")
            {
                var levelName =
                    $"Assets/_Prefabs/Levels/Level{_level.GetHashCode()}.prefab";
                EditorLevelInfo.Name = levelName;
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, levelName);
            }
            else
            {
                var level = PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, EditorLevelInfo.Name);
            }
            HiResScreenshot.TakeHiResShot($"Level{_level.GetHashCode()}");
            SceneManager.LoadSceneAsync("EditorMenu");
        }

        public void PlaceElement(Vector3 position, Orientation orientation, Direction direction)
        {
            var element = (GameObject)PrefabUtility.InstantiatePrefab(_currentObject, _level.transform);
            element.transform.position = position;

            if (element.transform.HasComponent<SlopeWalkable>(out var slope))
                slope.MatchRotation(orientation, direction);
            else if (element.CompareTag("PlayerSpawn"))
                EditorLevelInfo.PlayerSpawnPoint = element.transform.Find("Model").transform;

            if (element.transform.HasComponent<MaxCount>(out var max))
            {
                var theseObjects = _limitedObjects.Where(x => x.name == element.name || x.name.Contains(element.name)).ToArray();

                if (theseObjects.Length >= max.Max)
                {
                    var obj = theseObjects.First();
                    _limitedObjects.Remove(obj);
                    obj.SetActive(false);
                }

                _limitedObjects.Add(element);
            }

            if (element.transform.HasComponent<Colorable>(out var colorable))
                colorable.OcularState = _colorPalette.SelectedColor;
        }
    }
}

#endif
