using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Game;
using Misc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using OcularState = Game.GlassesController.OcularState;

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
            Invisible
        }

        public enum ColorableType
        {
            StateChanging,
            ColorChanging
        }

        [SerializeField] private ColorableType _type;

        [SerializeField, HideInInspector] private Color _color;
        [SerializeField, HideInInspector] private OcularState _ocularState;
        [SerializeField] private Material _blockMat;
        [SerializeField] private Material _outlineMat;
        [SerializeField] private GameObject _outlineModel;

        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;
        private GameObject[] _models;
        private static LevelInfo _levelInfo;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private OcularState _initialState;

        private BlockState _blockState;
        private GameObject _runtimeOutlineModel;

        private List<IController> _controllers;

        [DisableInPlayMode, ShowInInspector]
        public OcularState OcularState
        {
            get => _ocularState;
            set
            {
                var reset = value == OcularState.Null;
                _ocularState = reset ? _initialState : value;
                _color = InternalStateToColor(reset ? _initialState : value);

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
                        _renderers.ForEach(r =>
                        {
                            var tex = r.material.mainTexture;
                            r.material = _blockMat;
                            r.material.mainTexture = tex;
                        });
                        SetModelsState(true);

                        //todo alert player of collision/death
                        if (_blockState != BlockState.Visible)
                            if (Physics.Raycast(transform.position + (Vector3.up * 3), Vector3.down, out var hit, 2,
                                LayerMask.GetMask("Player")))
                            {
                                Debug.Log(hit.transform.name);
                                GameObject.FindWithTag("Player")?.GetComponent<Player.Player>().Death();
                            }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _blockState = value;
            }
        }

        private bool _outlined;
        public bool Outlined
        {
            get => _outlined;
            set
            {
                if (value)
                {
                    if (_runtimeOutlineModel == null)
                        _runtimeOutlineModel = Instantiate(_outlineModel, transform);

                    _runtimeOutlineModel.SetActive(true);
                }
                else if (_runtimeOutlineModel != null)
                {
                    _runtimeOutlineModel.SetActive(false);
                }

                _outlined = value;

            }
        }

        public void Initialize()
        {
            if (OcularState == OcularState.Z) return;
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
            if (OcularState == OcularState.Z)
                return (OcularState.Z, BlockState.Visible);

            var visible = IsColorVisible(OcularState);

            // ATM This code assumes controller is button bc thats the only implemented controller
            // TODO Come up with better controller solution

            if (_controllers.Count > 0)
            {
                if (((MonoBehaviour)_controllers[0]).GetComponent<Colorable>().State != BlockState.Visible)
                {
                    Outlined = false;
                    return visible
                        ? (_state: OcularState, BlockState.Visible)
                        : (_state: OcularState, BlockState.Invisible);
                }
                else
                {
                    Outlined = true;
                }
            }
            else
            {
                return visible
                    ? (_state: OcularState, BlockState.Visible)
                    : (_state: OcularState, BlockState.Invisible);
            }

            if (visible)
            {
                return (OcularState, BlockState.Visible);
            }

            if (OcularState == _initialState)
            {
                if (IsColorVisible(((ButtonWalkable)_controllers[0]).Color))
                {
                    return (((ButtonWalkable)_controllers[0]).Color, BlockState.Invisible);
                }

                return (OcularState, BlockState.Invisible);
            }

            return (OcularState, BlockState.Invisible);
        }

        private void InternalOnGlassesToggled()
        {
            switch (_type)
            {
                case ColorableType.StateChanging:
                    var (color, state) = CalculateVisibility();

                    State = state;

                    if (_runtimeOutlineModel != null)
                    {
                        var r = _runtimeOutlineModel.transform.GetChild(0).GetComponent<Renderer>();
                        r.GetPropertyBlock(_propBlock);
                        if (State != BlockState.Visible)
                        {
                            if (OcularState != _initialState)
                                _propBlock.SetColor("_Color", StateToColor(_initialState));
                            else
                                _propBlock.SetColor("_Color", StateToColor(((ButtonWalkable)_controllers[0]).Color));
                        }
                        else
                        {
                            if (OcularState != _initialState)
                                _propBlock.SetColor("_Color", StateToColor(_initialState));
                            else
                                _propBlock.SetColor("_Color", StateToColor(((ButtonWalkable)_controllers[0]).Color));
                        }
                        r.SetPropertyBlock(_propBlock);

                    }

                    if (transform.HasComponent<Walkable>(out var walkable))
                    {
                        var visible = _blockState == BlockState.Visible;

                        //if custom disable behavior gets overly complex, move into child classes
                        switch (walkable)
                        {
                            case ButtonWalkable button:
                                if (!visible)
                                {
                                    //button.State = false;
                                }

                                break;
                        }
                        walkable.CheckBelow(!visible);
                        walkable.Enabled = visible;
                    }
                    break;
                case ColorableType.ColorChanging:
                    OcularState = GlassesController.CurrentOcularState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Color StateToColor(OcularState ocularState)
        {
            if (ocularState == OcularState.Null)
                return Color.clear;

            switch (ocularState)
            {
                case OcularState.Z: return Color.white;
                case OcularState.A: return Color.red;
                case OcularState.AB: return new Color(1, .551f, 0);
                case OcularState.B: return new Color(1, 1, 0, 1);
                case OcularState.BC: return Color.green;
                case OcularState.C: return Color.blue;
                case OcularState.AC: return Color.magenta;
                case OcularState.Null: return Color.clear;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ocularState), ocularState, null);
            }
        }

        public static bool IsColorVisible(OcularState ocularState)
        {
            var c = ocularState;
            switch (GlassesController.CurrentOcularState)
            {
                case OcularState.Z:
                    return true;
                case OcularState.A: // Red
                    return c == OcularState.A;
                case OcularState.B: // Yellow
                    return c == OcularState.B;
                case OcularState.C: // Blue
                    return c == OcularState.C;
                case OcularState.AB: // Orange
                    return c == OcularState.AB || c == OcularState.A || c == OcularState.B;
                case OcularState.AC: // Magenta
                    return c == OcularState.AC || c == OcularState.A || c == OcularState.C;
                case OcularState.BC: // Green
                    return c == OcularState.BC || c == OcularState.B || c == OcularState.C;
                default:
                    return false;
            }
        }

        private Color InternalStateToColor(OcularState ocularState)
        {
            if (ocularState == OcularState.Null)
                return InternalStateToColor(_initialState);

            Color newCol;

            switch (ocularState)
            {
                case OcularState.Z: return Color.white;
                case OcularState.A: ColorUtility.TryParseHtmlString("#BE5151", out newCol); return newCol;
                case OcularState.AB: ColorUtility.TryParseHtmlString("#C99555", out newCol); return newCol;
                case OcularState.B: ColorUtility.TryParseHtmlString("#C7C360", out newCol); return newCol;
                case OcularState.BC: ColorUtility.TryParseHtmlString("#5BC75C", out newCol); return newCol;
                case OcularState.C: ColorUtility.TryParseHtmlString("#6E88CE", out newCol); return newCol;
                case OcularState.AC: ColorUtility.TryParseHtmlString("#975FBD", out newCol); return newCol;
                case OcularState.Null: return Color.clear;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ocularState), ocularState, null);
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
            _renderers = _models.Select(m => m.GetComponent<MeshRenderer>()).ToArray();
            if (Application.isPlaying)
            {
                _levelInfo = transform.parent.parent.GetComponent<LevelInfo>();
            }

            //#if UNITY_EDITOR
            _renderers.ForEach(r => r.sharedMaterial = _blockMat);
            //#endif

            _propBlock = new MaterialPropertyBlock();
            State = BlockState.Visible;
            OcularState = OcularState;
            SetModelsState(true);
        }

        private void Awake()
        {
            if (!Application.isPlaying) return;

            _controllers = new List<IController>();

            /*Addressables.LoadAssetAsync<GameObject>("block_outline_model").Completed += result =>
                {
                    _outlineModel = result.Result;
                };*/

            GlassesController.OnGlassesToggled += InternalOnGlassesToggled;
        }

        private void OnDestroy()
        {
            GlassesController.OnGlassesToggled -= InternalOnGlassesToggled;
        }
    }
}
