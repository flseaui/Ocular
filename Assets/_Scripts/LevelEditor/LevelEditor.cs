using System;
using System.Collections;
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

        private LevelController _levelController;
        
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
            
            PlayerPrefs.SetInt("PlayFromEditor", 1);
            
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
                var parent = _level.transform.parent;
                var level = parent.Find("Level");
                GameObject model = null;
                if (level) model = level.gameObject;

                var killZone = parent.Find("KillZone").gameObject;
                
                if (model != null)
                    model.SetActive(!model.activeSelf);
                if (killZone != null)
                    killZone.SetActive(!killZone.activeSelf);
            }
        }

        public void LoadLevel(GameObject level)
        {
            _level = ((GameObject) PrefabUtility.InstantiatePrefab(level)).transform.Find("MainFloor").gameObject;
            SetupLevel();
            SaveLevelInfo();
            DefaultLevelInfo();
        }

        private float? _savedBlockContrast,
            _savedColorIntensity,
            _savedShadowStrength;

        private Color? _savedShadowTint;
        
        public void NewLevel(int size)
        {
            _level = Instantiate(_levelBasePrefabs[size]).transform.Find("MainFloor").gameObject;
            _level.transform.parent.name = "BlankLevel";
            SetupLevel();
            DefaultLevelInfo();
        }

        private void DefaultLevelInfo()
        {
            EditorLevelInfo.BlockContrast = .5f;
            EditorLevelInfo.ColorIntensity = 1.2f;
            EditorLevelInfo.ShadowStrength = .3f;
            EditorLevelInfo.ShadowTint = Color.white;
        }

        private void SaveLevelInfo()
        {
            _savedBlockContrast = EditorLevelInfo.BlockContrast;
            _savedColorIntensity = EditorLevelInfo.ColorIntensity;
            _savedShadowStrength = EditorLevelInfo.ShadowStrength;
            _savedShadowTint = EditorLevelInfo.ShadowTint;
        }
        
        private void RestoreLevelInfo()
        {
            if (_savedBlockContrast.HasValue)
                EditorLevelInfo.BlockContrast = _savedBlockContrast.Value;
            if (_savedColorIntensity.HasValue)
                EditorLevelInfo.ColorIntensity = _savedColorIntensity.Value;
            if (_savedShadowStrength.HasValue)
                EditorLevelInfo.ShadowStrength = _savedShadowStrength.Value;
            if (_savedShadowTint.HasValue)
                EditorLevelInfo.ShadowTint = _savedShadowTint.Value;
        }
        
        private void SetupLevel()
        {
            _level.transform.parent.GetComponent<Animator>().enabled = false;
            EditorLevelInfo = _level.GetComponentInParent<LevelInfo>();
            _levelController = _gameManager.GetComponent<LevelController>();
            _levelController.LevelInfo = EditorLevelInfo;
            _level.transform.ForEachChild(x =>
            {
                if (x.HasComponent<MaxCount>())
                    _limitedObjects.Add(x.gameObject);
            });
            var editorCam = Camera.main.GetComponent<EditorCameraCenter>();
            editorCam.LevelInfo = EditorLevelInfo;
            editorCam.Init();
            _level.transform.parent.ForEachChild(child =>
            {
                if (child.CompareTag("LevelLight"))
                {
                    child.GetComponent<Light>().enabled = false;
                }
            });
        }
        
        public void TestLevel()
        {
            OnLevelPlayToggle?.Invoke(!_glassesContainer.activeSelf);
            //TODO Refactor the rest of these into their own scripts invoked by OnLevelPlayToggle
            _colorPalette.GetComponent<Animator>().SetBool(OnScreen, !_colorPalette.GetComponent<Animator>().GetBool(OnScreen));
            _objectDrawer.GetComponent<Animator>().SetBool(OnScreen, !_objectDrawer.GetComponent<Animator>().GetBool(OnScreen));
            _gameManager.gameObject.SetActive(!_gameManager.gameObject.activeSelf);
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
                _levelController.EntityManager.SpawnEntities();
            }
            else
            {
                _levelController.EntityManager.ClearEntities();
                _levelController.EntityManager.DespawnPlayer();
            }

        }
        
        public void RemoveObject(GameObject obj)
        {
            obj.SetActive(false);
        }
        
        public void SaveLevel()
        {
            RestoreLevelInfo();
            
            var levelGO = _level.transform.parent.Find("Level");
            if (levelGO != null)
            {
                levelGO.gameObject.SetActive(true);
            } 
            
            HiResScreenshot.TakeHiResShot("Level"+ _level.GetHashCode());

            StartCoroutine(FinishSave());
        }

        private IEnumerator FinishSave()
        {
            yield return new WaitUntil(() => !HiResScreenshot.TakingShot);
            
            _level.transform.parent.GetComponent<Animator>().enabled = true;
            _level.transform.parent.ForEachChild(child =>
            {
                if (child.CompareTag("LevelLight"))
                {
                    child.GetComponent<Light>().enabled = true;
                }
            });
            
            if (EditorLevelInfo.name == "BlankLevel")
            {
                var levelName =
                    $"Assets/_Prefabs/Levels/Level{_level.GetHashCode()}.prefab";
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, levelName);
            }
            else
            {
                var path = PlayerPrefs.GetString("LevelToLoad");
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, path);
            }
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
                colorable.QueueOcularStateChange(_colorPalette.SelectedColor);
        }
    }
}

#endif
