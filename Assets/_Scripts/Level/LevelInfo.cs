using System;
using System.Collections.Generic;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level
{
    public class LevelInfo : SerializedMonoBehaviour
    {
        public enum ColorSet
        {
            RGB,
            Custom
        }

        [SerializeField]
        public List<Color> BlockColors;

        [OnValueChanged(nameof(ApplyColorPreset))]
        public ColorSet ColorPreset;

        //[OnValueChanged(nameof(CalcBlockColors))]
        public List<Glasses> LevelGlasses;

        public int NumConcurrentGlasses;

        public Transform PlayerSpawnPoint;

        public string Name;

        private void ApplyColorPreset()
        {
            if (ColorPreset == ColorSet.RGB)
            {

                var yellow = new Color(1, 1, 0, 1);
                var orange = new Color(1, .551f, 0);

                LevelGlasses = new List<Glasses>
                {
                    new Glasses(Color.red, GlassesType.One,KeyCode.Q),
                    new Glasses(yellow, GlassesType.Two, KeyCode.W),
                    new Glasses(Color.blue, GlassesType.Three, KeyCode.E)
                };

                BlockColors = new List<Color>
                {
                    Color.white,
                    Color.red,
                    yellow,
                    Color.blue,
                    orange,
                    Color.magenta,
                    Color.green
                };
            }
        }
    }
}
/*private void CalcBlockColors()
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
}    */
    