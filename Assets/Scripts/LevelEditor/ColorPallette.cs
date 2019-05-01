using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class ColorPallette : MonoBehaviour
{
    public Color SelectedColor = Color.white;

    private ColorSwatch[] _colorSwatches;

    private void Awake()
    {
        _colorSwatches = GetComponentsInChildren<ColorSwatch>();
    }

    public void SetColor(Color color)
    {
        SelectedColor = color;
        _colorSwatches.Where(x => x.Color != color).ForEach(x => x.Deselect());
    }
}
