//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Game;
//using Misc;
//using Sirenix.OdinInspector;
//using Sirenix.Utilities;
//using UnityEngine;
//using UnityEngine.Experimental.UIElements.StyleEnums;
//
//namespace Level.Objects {
//    [ExecuteInEditMode]
//    public class Colorable : MonoBehaviour
//    {
//        [SerializeField] private Material _blockMat;
//
//        [SerializeField, HideInInspector] private Color _color;
//
//        private Color _initialColor;
//        private LevelInfo _levelInfo;
//
//        private GameObject[] _models;
//
//        private bool _outlined;
//
//        [SerializeField] private Material _outlineMat;
//
//        private MaterialPropertyBlock _propBlock;
//        private Renderer[] _renderers;
//        private bool _isTarget;
//        private Color _altColor;
//        private bool _tempColor;
//        private List<bool> _controllers;
//
//        public Action<bool> OnVisibilityChanged;
//
//        public void SetControllerState(int controllerIndex, bool state)
//        {
//            if (_controllers.Count <= controllerIndex) return;
//            
//            _controllers[controllerIndex] = state;
//        }
//        
//        public int RegisterController()
//        {
//            _controllers.Add(new bool());
//            return _controllers.Count;
//        }
//        
//        public void MarkAsTarget(Color color)
//        {
//            _isTarget = true;
//            _altColor = color;
//        }
//        
//        [ShowInInspector]
//        public bool Outlined
//        {
//            get => _outlined;
//            set
//            {
//                _outlined = value;
//                if (value) _renderers.ForEach(r => r.material = _outlineMat);
//                else _renderers.ForEach(r => r.material = _blockMat);
//            }
//        }
//
//        [ColorPalette("RGB"), ShowInInspector]
//        public Color Color
//        {
//            get => _color;
//            set
//            {
//                _color = value;
//                _renderers.ForEach(r => r.GetPropertyBlock(_propBlock));
//                _propBlock.SetColor("_Color", value);
//                _renderers.ForEach(r => r.SetPropertyBlock(_propBlock));
//            }
//        }
//
//        private void SetTempColor(Color color)
//        {
//            _tempColor = true;
//            _renderers.ForEach(r => r.GetPropertyBlock(_propBlock));
//            _propBlock.SetColor("_Color", color);
//            _renderers.ForEach(r => r.SetPropertyBlock(_propBlock));
//        }
//        
//        public void ToggleColor()
//        {
//            if (!_isTarget)
//                return;
//
//            Color = Color == _altColor ? _initialColor : _altColor;
//            
//            if (Color == Color.white) return;
//
//            var visible =
//                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements
//                    .Contains(GlassesController.CurrentGlassesColor) ?? false;
//
//            if (transform.HasComponent<Walkable>(out var walkable))
//            {
//                walkable.CheckBelow(!visible);
//                walkable.Enabled = visible;
//            }
//
//            UpdateOutline(visible);
//        }
//
//        private void UpdateOutline(bool visibility)
//        {
//            if (!visibility || !_controllers.Contains(false))
//            {
//                if (Color == _initialColor)
//                {
//                    SetTempColor(_altColor);
//                    Outlined = true;
//                }
//                else if (Color == _altColor)
//                {
//                    SetTempColor(_initialColor);
//                    Outlined = true;
//                }
//            }
//            else
//            {
//                Outlined = false;
//                if (_tempColor)
//                {
//                    _tempColor = false;
//                    Color = Color;
//                }
//            }
//        }
//        
//        private void OnEnable()
//        {
//            var temp = new List<GameObject>();
//            foreach (Transform child in transform)
//                if (child.CompareTag("Colorable"))
//                    temp.Add(child.gameObject);
//            _models = temp.ToArray();
//            _renderers = _models.Select(m => m.GetComponent<Renderer>()).ToArray();
//            if (Application.isPlaying)
//                _levelInfo = transform.parent.parent.GetComponent<LevelInfo>();
//            _propBlock = new MaterialPropertyBlock();
//            Color = Color;
//            SetModelsState(true);
//        }
//
//        public void Initialize()
//        {
//            if (Color == Color.white) return;
//            SetModelsState(false);
//            if (transform.HasComponent<Walkable>(out var walkable))
//            {
//                walkable.CheckBelow(walkable.Enabled);
//                walkable.Enabled = false;
//            }
//        }
//
//        private void Awake()
//        {
//            if (!Application.isPlaying) return;
//
//           GlassesController.OnGlassesToggled += InternalOnGlassesToggled;
//            _controllers = new List<bool>();
//            _isTarget = false;
//        }
//
//        private void InternalOnGlassesToggled(Color color)
//        {
//            if (Color == Color.white) return;
//
//            var visible =
//                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements
//                    .Contains(color) ?? false;
//
//            if (_models[0].activeSelf == visible) return;
//            
//            if (transform.HasComponent<Walkable>(out var walkable))
//            {
//                //put custom disable behavior here
//                switch (walkable) 
//                {
//                    case ButtonWalkable button:
//                        if (!visible)
//                        {
//                            button.State = false;
//                        }
//
//                        break;
//                }
//                    
//                walkable.CheckBelow(!visible);
//                walkable.Enabled = visible;
//            }
//            
//            OnVisibilityChanged?.Invoke(visible);
//            
//            if (_isTarget)
//                UpdateOutline(visible);
//            
//            if (!Outlined)
//                SetModelsState(visible);
//        }
//        
//        private void Start()
//        {
//            _initialColor = Color;
//            OnVisibilityChanged?.Invoke(false);
//        }
//        
//        private void OnDestroy()
//        {
//            GlassesController.OnGlassesToggled -= InternalOnGlassesToggled;
//        }
//
//        private void SetModelsState(bool state)
//        {
//            _models.ForEach(m => m.SetActive(state));
//        }
//    }
//}