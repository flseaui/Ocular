using System.Collections.Generic;
using System.Linq;
using Level.Objects;
using Misc;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        private ColorPalette _colorPalette;
        private GameObject _currentObject;
        private GameObject _level;

        private List<GameObject> _limitedObjects;

        private void Awake()
        {
            _limitedObjects = new List<GameObject>();
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