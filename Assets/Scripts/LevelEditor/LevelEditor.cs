using Level.Objects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        private GameObject _currentObject;
        private GameObject _level;
        private ColorPalette _colorPalette;

        private void Awake()
        {
            _colorPalette = GameObject.Find("ColorPalette").GetComponent<ColorPalette>();
            ObjectDrawer.OnObjectSelectionChanged += @object => { _currentObject = @object; };
        }
        
        private void Start()
        {
            CreateLevel();
        }
        
        public void CreateLevel()
        {
            Addressables.LoadAsset<GameObject>("blank_level").Completed += handle =>
            {
                _level = Instantiate(handle.Result).transform.Find("MainFloor").gameObject;
                GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            };
        }

        public void PlaceObject(Vector3 position, Quaternion rotation)
        {
            var @object = Instantiate(_currentObject, position, rotation, _level.transform);
            @object.GetComponent<Colorable>().Color = _colorPalette.SelectedColor;
        }
    }
}