using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelEditor;
using Sirenix.Utilities;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public Color SelectedColor;

    private ObjectPlacer _placer;
    private ColorSwatch[] _colorSwatches;

    public Action<Color> OnColorChanged;
    
    private void Awake()
    {
        _colorSwatches = GetComponentsInChildren<ColorSwatch>();
        _placer = GameObject.Find("Placer").GetComponent<ObjectPlacer>();
    }

    private void Start()
    {
        _colorSwatches[0].Select();
    }
    
    public void SetColor(Color color)
    {
        SelectedColor = color;
        _colorSwatches.Where(x => x.Color != color).ForEach(x => x.Deselect());
        _placer.SetSelectedObjectsColor(color);
        OnColorChanged?.Invoke(SelectedColor);
    }
}
