using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public Color SelectedColor;

    private ColorSwatch[] _colorSwatches;

    private void Awake()
    {
        _colorSwatches = GetComponentsInChildren<ColorSwatch>();
    }

    private void Start()
    {
        _colorSwatches[0].Select();
    }
    
    public void SetColor(Color color)
    {
        SelectedColor = color;
        _colorSwatches.Where(x => x.Color != color).ForEach(x => x.Deselect());
    }
}
