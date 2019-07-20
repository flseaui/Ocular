#if UNITY_EDITOR

using System;
using System.Linq;
using Game;
using Sirenix.Utilities;
using UnityEngine;

namespace OldEditor
{
    public class ColorPalette : MonoBehaviour
    {
        public OcularState SelectedColor;

        private ObjectPlacer _placer;
        private ColorSwatch[] _colorSwatches;

        public Action<OcularState> OnColorChanged;
    
        private void Awake()
        {
            _colorSwatches = GetComponentsInChildren<ColorSwatch>();
            _placer = GameObject.Find("Placer").GetComponent<ObjectPlacer>();
        }

        private void Start()
        {
            _colorSwatches[0].Select();
        }
    
        public void SetColor(OcularState color)
        {
            SelectedColor = color;
            _colorSwatches.Where(x => x.Color != color).ForEach(x => x.Deselect());
            _placer.SetSelectedObjectsColor(color);
            OnColorChanged?.Invoke(SelectedColor);
        }
    }
}

#endif