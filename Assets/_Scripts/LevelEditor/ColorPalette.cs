#if UNITY_EDITOR

using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using OcularState = Game.GlassesController.OcularState;

namespace LevelEditor
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
            _colorSwatches.Where(x => x.Color == SelectedColor).ForEach(x => x.Select());
            _colorSwatches.Where(x => x.Color != SelectedColor).ForEach(x => x.Deselect());
            //_colorSwatches[0].Select();
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