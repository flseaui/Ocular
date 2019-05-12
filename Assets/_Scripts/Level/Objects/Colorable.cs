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
    /// <summary>
    /// Represents the color of an object, can be modified in editor and at runtime.
    /// </summary>
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
        [SerializeField, HideInInspector] private OcularState _ocularState;
        [SerializeField] private Material _blockMat;
        [SerializeField] private Material _outlineMat;
        
        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;
        private GameObject[] _models;
        private static LevelInfo _levelInfo;
        private OcularState _initialState;
        private BlockState _blockState;

        private List<IController> _controllers;

        public OcularState OcularState
        {
            get => _ocularState;
            set
            {
                _ocularState = value;
                _color = StateToColor(value) == Color.clear ? StateToColor(_initialState) : StateToColor(value);

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
            if (OcularState == OcularState.Zero) return;
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
        private (OcularState color, BlockState state) CalculateVisibility()
        {
            if (OcularState == OcularState.Zero) 
                return (OcularState.Zero, BlockState.Visible);

            var visible = IsColorVisible(OcularState);

            // ATM This code assumes controller is button bc thats the only implemented controller
            // TODO Come up with better controller solution

            if (_controllers.All(c => ((MonoBehaviour) c).GetComponent<Colorable>().State != BlockState.Visible))
                return visible ? (_state: OcularState, BlockState.Visible) : (_state: OcularState, BlockState.Invisible);
            
            if (visible)
                return  (OcularState, BlockState.Visible);

            if (OcularState == _initialState)
            {
                if (IsColorVisible(((ButtonWalkable) _controllers[0]).Color))
                    return (((ButtonWalkable) _controllers[0]).Color, BlockState.Outlined);
                return (OcularState, BlockState.Invisible);
            }

            return (OcularState, BlockState.Invisible);
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
                    _propBlock.SetColor("_Color", StateToColor(color));
                    r.SetPropertyBlock(_propBlock);
                }
            }
            else
                OcularState = color;

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

        public static Color StateToColor(OcularState ocularState)
        {
            if (ocularState == OcularState.Null)
                return Color.clear;
            
            return _levelInfo.BlockColors[(int) ocularState];
        }
        
        public static bool IsColorVisible(OcularState ocularState)
        {
            var c = GlassesController.CurrentOcularState;
            switch (ocularState)
            {
                case OcularState.Zero:
                    return true;
                case OcularState.One:
                    return c == OcularState.One || c == OcularState.Two || c == OcularState.Five;
                case OcularState.Two:
                    return c == OcularState.Two;
                case OcularState.Three:
                    return c == OcularState.Three || c == OcularState.Five;
                case OcularState.Four:
                    return c == OcularState.Four;
                case OcularState.Five:
                    return c == OcularState.Five;
                case OcularState.Six:
                    return c == OcularState.Two || c == OcularState.Six;
                default:
                    return false;
            }

        }
        
        private void SetModelsState(bool state)
        {
            foreach (var model in _models)
                model.SetActive(state);
        }
        
        private void Start()
        {
            _initialState = OcularState;
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
            OcularState = OcularState;
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