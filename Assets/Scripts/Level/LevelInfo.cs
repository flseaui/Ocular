using System;
using System.Collections.Generic;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level {
    public class LevelInfo : SerializedMonoBehaviour
    {
        public enum ColorSet
        {
            RGB,
            Custom
        }

        public List<ColorData> BlockColors;

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
                LevelGlasses = new List<Glasses>
                {
                    new Glasses(Color.red, KeyCode.Q),
                    new Glasses(Color.green, KeyCode.W),
                    new Glasses(Color.blue, KeyCode.E)
                };

                var yellow = new Color(1, 1, 0, 1);
                BlockColors = new List<ColorData>
                {
                    new ColorData(Color.red, new List<Color> {Color.red, Color.magenta, yellow}),
                    new ColorData(Color.green, new List<Color> {Color.green, Color.cyan, yellow}),
                    new ColorData(Color.blue, new List<Color> {Color.blue, Color.cyan, Color.magenta}),
                    new ColorData(Color.cyan, new List<Color> {Color.cyan}),
                    new ColorData(Color.magenta, new List<Color> {Color.magenta}),
                    new ColorData(yellow, new List<Color> {yellow})
                };
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

        [Serializable]
        public class ColorData
        {
            public Color Color;
            public List<Color> Requirements;

            public ColorData(Color color, List<Color> requirements)
            {
                Color = color;
                Requirements = requirements;
            }
        }
    }
}