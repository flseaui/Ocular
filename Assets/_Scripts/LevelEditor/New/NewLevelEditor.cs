using System;
using Level;
using Level.Objects;
using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LevelEditor.New
{
    public class NewLevelEditor : MonoBehaviour
    {
        public GameObject[,,] _currentLevel;

        private ColorPalette _colorPalette;
        private ObjectDrawer _objectDrawer;
        private GameObject _currentObject;
        private GameObject _level;

        private readonly Vector3Int _levelDimensions = new Vector3Int(100, 100, 100);
        
        private void Awake()
        {
            _currentLevel = new GameObject[_levelDimensions.x, _levelDimensions.y, _levelDimensions.z];
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

        
        public void PlaceElement(Vector3Int position, Orientation orientation, Direction direction)
        {
            var element = Instantiate(_currentObject, position, Quaternion.identity, _level.transform);
            
            if (element.transform.HasComponent<SlopeWalkable>(out var slope))
                slope.MatchRotation(orientation, direction);

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