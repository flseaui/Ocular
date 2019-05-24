using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using LevelEditor;
using LevelEditor.New;
using Sirenix.Utilities;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public OcularState SelectedColor;

    private NewObjectPlacer _placer;
    private ColorSwatch[] _colorSwatches;

    public Action<OcularState> OnColorChanged;
    
    private void Awake()
    {
        _colorSwatches = GetComponentsInChildren<ColorSwatch>();
        _placer = GameObject.Find("Placer").GetComponent<NewObjectPlacer>();
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
