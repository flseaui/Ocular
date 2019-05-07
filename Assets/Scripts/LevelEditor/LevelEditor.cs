using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using Level;
using Level.Objects;
using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        private ColorPalette _colorPalette;
        private ObjectDrawer _objectDrawer;
        private GameObject _currentObject;
        private GameObject _level;

        [SerializeField] GameManager _gameManager;
        [SerializeField] GameObject _glassesContainer;
        
        private List<GameObject> _limitedObjects;
        private static readonly int OnScreen = Animator.StringToHash("OnScreen");

        public static Action<bool> OnLevelPlayToggle;
        
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
            if (level == "New")
            {
                NewLevel();
            }
            else
            {
                var levelGameObject = (GameObject) AssetDatabase.LoadAssetAtPath(level, typeof(GameObject));
                LoadLevel(levelGameObject);
            }
        }

        public void LoadLevel(GameObject level)
        {
            _level = Instantiate(level).transform.Find("MainFloor").gameObject;
            GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
        }

        public void NewLevel()
        {
            Addressables.LoadAsset<GameObject>("blank_level").Completed += handle =>
            {
                _level = Instantiate(handle.Result).transform.Find("MainFloor").gameObject;
                _level.transform.parent.GetComponent<LevelInfo>().Name = "BlankLevel";
                GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            };
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
                    $"Assets/Prefabs/Levels/Level{_level.GetHashCode()}.prefab";
                _level.transform.parent.GetComponent<LevelInfo>().Name = levelName;
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, levelName);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(_level.transform.parent.gameObject, _level.transform.parent.GetComponent<LevelInfo>().Name);
            }

            SceneManager.LoadSceneAsync("EditorMenu");
        }
        
        public void PlaceObject(Vector3 position, Orientation orientation, Direction direction)
        {
            var @object = Instantiate(_currentObject, position, Quaternion.identity, _level.transform);
            if (@object.transform.HasComponent<SlopeWalkable>(out var slope))
            {
                slope.MatchRotation(orientation, direction);
            }
            
            if (@object.transform.HasComponent<MaxCount>(out var max))
            {
                var objects = _limitedObjects.Count > 0
                    ? _limitedObjects.Where(x =>
                        x.name == @object.name || x.name.Substring(0, x.name.IndexOf("(")) == @object.name)
                    : null;
                if (objects != null && objects.Count() >= max.Max)
                {
                    _limitedObjects.Remove(@object);
                    Destroy(@object);
                }
                else
                {
                    _limitedObjects.Add(@object);
                    if (@object.transform.HasComponent<Colorable>(out var colorable))
                        colorable.Color = _colorPalette.SelectedColor;
                }
            }
            else
            {
                if (@object.transform.HasComponent<Colorable>(out var colorable))
                    colorable.Color = _colorPalette.SelectedColor;
            }
        }
    }
}