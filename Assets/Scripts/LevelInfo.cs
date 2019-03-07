using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class LevelInfo : SerializedMonoBehaviour
{
    [OnValueChanged(nameof(CalcBlockColors))]
    public List<Glasses> LevelGlasses;

    private void CalcBlockColors()
    {
        BlockColors.Clear();
        foreach (var glassesX in LevelGlasses)
        {
            foreach (var glassesY in LevelGlasses)
            {
                var color = new Color(
                    (glassesX.Color.r + glassesY.Color.r) / 2,
                    (glassesX.Color.g + glassesY.Color.g) / 2,
                    (glassesX.Color.b + glassesY.Color.b) / 2);
                if (!BlockColors.Contains(color))
                    BlockColors.Add(color);
            }
        }
    }
    
    public List<Color> BlockColors;
}
