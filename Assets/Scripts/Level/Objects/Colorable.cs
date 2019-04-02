using System.Collections.Generic;
using System.Linq;
using Game;
using Misc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Level.Objects {
    [ExecuteInEditMode]
    public class Colorable : MonoBehaviour
    {
        [SerializeField] private Material _blockMat;

        [SerializeField, HideInInspector] private Color _color;

        private Color _initialColor;
        private LevelInfo _levelInfo;

        private GameObject[] _models;

        private bool _outlined;

        [SerializeField] private Material _outlineMat;

        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;

        [ShowInInspector]
        public bool Outlined
        {
            get => _outlined;
            set
            {
                _outlined = value;
                if (value) _renderers.ForEach(r => r.material = _outlineMat);
                else _renderers.ForEach(r => r.material = _blockMat);
            }
        }

        [ColorPalette("RGB"), ShowInInspector]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                _renderers.ForEach(r => r.GetPropertyBlock(_propBlock));
                _propBlock.SetColor("_Color", value);
                _renderers.ForEach(r => r.SetPropertyBlock(_propBlock));
            }
        }

        public void ChangeColorWithOutline(Color color)
        {
            Color = color;

            if (Color == Color.white) return;

            var visible =
                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements
                    .Contains(GlassesController.CurrentGlassesColor) ?? false;

            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(!visible);
                walkable.Enabled = visible;
            }

            Outlined = !visible;
        }

        public void ResetColorFromOutline()
        {
            ChangeColorWithOutline(_initialColor);
        }

        private void OnEnable()
        {
            var temp = new List<GameObject>();
            foreach (Transform child in transform)
                if (child.CompareTag("Colorable"))
                    temp.Add(child.gameObject);
            _models = temp.ToArray();
            _renderers = _models.Select(m => m.GetComponent<Renderer>()).ToArray();
            if (Application.isPlaying)
                _levelInfo = transform.parent.parent.GetComponent<LevelInfo>();
            _propBlock = new MaterialPropertyBlock();
            Color = Color;
            SetModelsState(true);
        }

        public void Initialize()
        {
            if (Color == Color.white) return;
            SetModelsState(false);
            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(walkable.Enabled);
                walkable.Enabled = false;
            }
        }

        private void Awake()
        {
            if (!Application.isPlaying) return;

            GlassesController.OnGlassesToggled += InternalOnGlassesToggled;
        }

        private void InternalOnGlassesToggled(Color color)
        {
            if (Color == Color.white) return;

            var visible =
                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements
                    .Contains(color) ?? false;

            if (_models[0].activeSelf == visible) return;

            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(!visible);
                walkable.Enabled = visible;
            }

            SetModelsState(visible);
        }
        
        private void Start()
        {
            _initialColor = Color;
        }

        private void OnDestroy()
        {
            GlassesController.OnGlassesToggled -= InternalOnGlassesToggled;
        }

        private void SetModelsState(bool state)
        {
            _models.ForEach(m => m.SetActive(state));
        }
    }
}