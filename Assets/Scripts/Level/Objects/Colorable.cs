using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Misc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Level.Objects
{
    [ExecuteInEditMode]
    public class Colorable : MonoBehaviour
    {
        public enum BlockState
        {
            Visible,
            Invisible,
            Outlined
        }
        
        [SerializeField, HideInInspector] private Color _color;
        [SerializeField] private Material _blockMat;
        [SerializeField] private Material _outlineMat;
        
        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;
        private GameObject[] _models;
        private LevelInfo _levelInfo;
        private Color _initialColor;
        private BlockState _blockState;

        private List<IController> _controllers;
        
        [ColorPalette("RGB"), ShowInInspector]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value == Color.clear ? _initialColor : value;

                foreach (var r in _renderers)
                {
                    r.GetPropertyBlock(_propBlock);
                    _propBlock.SetColor("_Color", _color);
                    r.SetPropertyBlock(_propBlock);
                }
            }
        }

        public BlockState State
        {
            get => _blockState;
            set
            {
                if (_blockState == value)
                    return;
                switch (value)
                {
                    case BlockState.Invisible:
                        SetModelsState(false);
                        break;
                    case BlockState.Visible:
                        _renderers.ForEach(r => r.material = _blockMat);
                        SetModelsState(true);
                        break;
                    case BlockState.Outlined:
                        _renderers.ForEach(r => r.material = _outlineMat);
                        SetModelsState(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _blockState = value;
            }
        }

        public void Initialize()
        {
            if (Color == Color.white) return;
            State = BlockState.Invisible;
            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(walkable.Enabled);
                walkable.Enabled = false;
            }
        }

        public void RegisterController(IController controller)
        {
            _controllers.Add(controller);
        }

        public void UpdateState() => InternalOnGlassesToggled();
        
        /// <summary>
        /// Calculates the color and visibility of the <c>Colorable</c> based on its properties and environment.
        /// </summary>
        /// <returns>
        /// A tuple containing the color and state of a colorable.
        /// </returns>
        // TODO Only calculate once per color then update all colorables, continue using custom logic for controlled blocks
        private (Color color, BlockState state) CalculateVisibility()
        {
            if (Color == Color.white) 
                return (Color.white, BlockState.Visible);

            var visible = IsColorVisible(Color);

            // ATM This code assumes controller is button bc thats the only implemented controller
            // TODO Come up with better controller solution

            if (_controllers.All(c => ((MonoBehaviour) c).GetComponent<Colorable>().State != BlockState.Visible))
                return visible ? (Color, BlockState.Visible) : (Color, BlockState.Invisible);
            
            if (visible)
                return  (Color, BlockState.Visible);

            if (Color == _initialColor)
            {
                if (IsColorVisible(((ButtonWalkable) _controllers[0]).Color))
                    return (((ButtonWalkable) _controllers[0]).Color, BlockState.Outlined);
                return (Color, BlockState.Invisible);
            }

            return (Color, BlockState.Invisible);
        }
        
        private void InternalOnGlassesToggled()
        {            
            var (color, state) = CalculateVisibility();
    
            State = state;
            if (state == BlockState.Outlined)
            {
                // Set temp color for outline
                foreach (var r in _renderers)
                {
                    r.GetPropertyBlock(_propBlock);
                    _propBlock.SetColor("_Color", color);
                    r.SetPropertyBlock(_propBlock);
                }
            }
            else
                Color = color;

            if (transform.HasComponent<Walkable>(out var walkable))
            {
                var visible = _blockState == BlockState.Visible;
                
                //if custom disable behavior gets overly complex, move into child classes
                switch (walkable) 
                {
                    case ButtonWalkable button:
                        if (!visible)
                        {
                            button.State = false;
                        }

                        break;
                }
                    
                walkable.CheckBelow(!visible);
                walkable.Enabled = visible;
            }
        }

        private bool IsColorVisible(Color color)
        {
            return _levelInfo.BlockColors.FirstOrDefault(x => x.Color == color)?.Requirements
                        .Contains(GlassesController.CurrentGlassesColor) ?? false;
        }
        
        private void SetModelsState(bool state)
        {
            foreach (var model in _models)
                model.SetActive(state);
        }
        
        private void Start()
        {
            _initialColor = Color;
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

        private void Awake()
        {
            if (!Application.isPlaying) return;

            _controllers = new List<IController>();
            
            GlassesController.OnGlassesToggled += InternalOnGlassesToggled;
        }
        
        private void OnDestroy()
        {
            GlassesController.OnGlassesToggled -= InternalOnGlassesToggled;
        }
    }
}