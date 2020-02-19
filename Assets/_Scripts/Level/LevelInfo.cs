using Game;
using Misc;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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
        public string InGameName;

        public Animator Animator;

        public bool ReadyToLoad;

        [BoxGroup("Camera")] public bool HasCustomCamera;
        [BoxGroup("Camera")] public Vector3 CameraPosition;
        [BoxGroup("Camera")] public float CameraSize;
        [BoxGroup("Camera")] public Transform CameraCenter;
        [BoxGroup("Camera")] public Direction CameraStartDirection;
        [BoxGroup("Camera")] public bool HasCustomConstants;
        [BoxGroup("Camera")] public float ZoomConstantX;
        [BoxGroup("Camera")] public float ZoomConstantY;
        

        [BoxGroup("Fog")] public bool HasCustomFog;
        [BoxGroup("Fog")] public float FogHeightStart;
        [BoxGroup("Fog")] public float FogHeightEnd;
        [BoxGroup("Fog")] public float FogPlaneY;
        [BoxGroup("Fog")] public bool HasCustomFogColor;
        [BoxGroup("Fog")] public Color FogColor;

        [Button, BoxGroup("Camera")]
        private void SetCamInfo()
        {
            if (Camera.main == null) return;

            var main = Camera.main;

            CameraPosition = main.transform.localPosition;
            CameraSize = main.orthographicSize;
        }

        [Button, BoxGroup("Fog")]
        private void SetFogInfo()
        {
            var fog = GameObject.Find("Height Fog Global").GetComponent<HeightFogGlobal>();
            var plane = GameObject.Find("FauxFogPlane");

            HasCustomFog = true;
            FogHeightStart = fog.fogHeightStart;
            FogHeightEnd = fog.fogHeightEnd;
            FogPlaneY = plane.transform.position.y;
            FogColor = fog.fogColor;
        }
        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void ApplyColorPreset()
        {
            if (ColorPreset == ColorSet.RGB)
            {

                var yellow = new Color(1, 1, 0, 1);
                var orange = new Color(1, .551f, 0);

                LevelGlasses = new List<Glasses>
                {
                    new Glasses(Color.red, GlassesType.One),
                    new Glasses(yellow, GlassesType.Two),
                    new Glasses(Color.blue, GlassesType.Three)
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
