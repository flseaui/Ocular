using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Game;
using Level.Objects.Clones;
using Misc;
using OcularAnimation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using OcularState = Game.GlassesController.OcularState;

namespace Level.Objects
{
    /// <summary>
    /// Represents the color of an object, can be modified in editor and at runtime.
    /// </summary>
    [ExecuteInEditMode]
    public class Colorable : MonoBehaviour
    {
        public enum BlockStateEnum
        {
            Visible,
            Invisible
        }

        public enum ColorableType
        {
            StateChanging,
            ColorChanging
        }
        
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int ColorProp = Shader.PropertyToID("_Color");
        private static readonly int ShadowStrength = Shader.PropertyToID("_ShadowStrength");
        private static readonly int ShadowTint = Shader.PropertyToID("_ShadowTint");
        
        [SerializeField] private ColorableType _type;

        [SerializeField, HideInInspector] private Color _color;
        [SerializeField, HideInInspector, FormerlySerializedAs("_ocularState")] private OcularState _ocularColor;
        [SerializeField] private bool _entity;
        [SerializeField] private bool _alwaysVisible;
        [SerializeField] private bool _dontUseBlockMat;
        [SerializeField] private Material _blockMat;
        private GameObject _outlineModel;

        private MaterialPropertyBlock _propBlock;
        private MeshRenderer[] _renderers;
        private GameObject[] _models;
        private LevelInfo _levelInfo;

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        private OcularState _initialColor;

        private BlockStateEnum _blockState;
        private GameObject _runtimeOutlineModel;
        private bool _firstTimeOutlined;

        public List<IController> Controllers;

        [DisableInPlayMode, ShowInInspector]
        public OcularState OcularColor
        {
            get => _ocularColor;
            set
            {
                var reset = value == OcularState.Null;
                _ocularColor = reset ? _initialColor : value;
                _color = InternalStateToColor(reset ? _initialColor : value);

                var rendererCount = _renderers.Length;
                for (var i = 0; i < rendererCount; i++)
                {
                    var r = _renderers[i];
                    r.GetPropertyBlock(_propBlock);
                    _propBlock.SetColor(ColorProp, _color);
                    r.SetPropertyBlock(_propBlock);
                }
            }
        }

        [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public BlockStateEnum BlockState
        {
            get => _blockState;
            set
            {
                if (_blockState == value)
                    return;
                switch (value)
                {
                    case BlockStateEnum.Invisible:
                        if (_entity)
                        {
                            //TODO un hard code for clone
                            GetComponent<Clone>().GoInvisible();
                        }
                        else
                        {
                            SetModelsState(false);
                        }

                        break;
                    case BlockStateEnum.Visible:
                        if (!_dontUseBlockMat)
                        {
                            var rendererCount = _renderers.Length;
                            for (var i = 0; i < rendererCount; ++i)
                            {
                                var r = _renderers[i];
                                var tex = r.material.mainTexture;
                                r.material = _blockMat;
                                r.material.mainTexture = tex;
                            }
                        }

                        if (_entity)
                        {
                            //TODO un hard code for clone
                            GetComponent<Clone>().GoVisible();
                        }
                        else
                        {
                            SetModelsState(true);
                        }

                        //todo alert player of collision/death
                        if (_blockState != BlockStateEnum.Visible)
                        {
                            if (Physics.Raycast(transform.position + (Vector3.up * 3), Vector3.down, out var hit, 3,
                                LayerMask.GetMask("Player")))
                            {
                                //janky fix?
                                if (PauseMenu.Restarting)
                                    break;
                                if (hit.transform.HasComponent<Player.Player>(out var player))
                                {
                                    player.Death();
                                }
                                else if (hit.transform.HasComponent<Clone>(out var clone))
                                {
                                    if (!_entity && !GetComponent<CloneSpawn>())
                                        clone.Death();
                                }
                            }
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
            if (OcularColor == OcularState.Z || _type == ColorableType.ColorChanging) return;
            
            BlockState = BlockStateEnum.Invisible;

            if (!transform.HasComponent<Walkable>(out var walkable)) return;
            
            walkable.CheckBelow(walkable.Enabled);
            walkable.Enabled = false;
        }

        public void RegisterController(IController controller)
        {
            Controllers.Add(controller);
        }

        public void UpdateState() => InternalOnGlassesToggled();

        /// <summary>
        /// Calculates the color and visibility of the <c>Colorable</c> based on its properties and environment.
        /// </summary>
        /// <returns>
        /// A tuple containing the color and state of a colorable.
        /// </returns>
        // TODO Only calculate once per color then update all colorables, continue using custom logic for controlled blocks
        private (OcularState color, BlockStateEnum state) CalculateVisibility()
        {
            if (OcularColor == OcularState.Z)
                return (OcularState.Z, BlockStateEnum.Visible);

            var visible = IsColorVisible(OcularColor);

            // ATM This code assumes controller is button bc thats the only implemented controller
            // TODO Come up with better controller solution

            if (Controllers.Count > 0)
            {
                if (((MonoBehaviour)Controllers[0]).GetComponent<Colorable>().BlockState != BlockStateEnum.Visible)
                {
                    Outlined = false;
                    return visible
                        ? (_state: OcularColor, BlockStateEnum.Visible)
                        : (_state: OcularColor, BlockStateEnum.Invisible);
                }
                else
                {
                    Outlined = true;
                }
            }
            else
            {
                return visible
                    ? (_state: OcularColor, BlockStateEnum.Visible)
                    : (_state: OcularColor, BlockStateEnum.Invisible);
            }

            if (visible)
            {
                return (OcularColor, BlockStateEnum.Visible);
            }

            if (OcularColor == _initialColor)
            {
                if (IsColorVisible(((ButtonWalkable)Controllers[0]).Color))
                {
                    return (((ButtonWalkable)Controllers[0]).Color, BlockStateEnum.Invisible);
                }

                return (OcularColor, BlockStateEnum.Invisible);
            }

            return (OcularColor, BlockStateEnum.Invisible);
        }

        private void InternalOnGlassesToggled()
        {
            switch (_type)
            {
                case ColorableType.StateChanging:
                    var (color, state) = CalculateVisibility();

                    BlockState = _alwaysVisible ? BlockStateEnum.Visible : state;

                    if (_runtimeOutlineModel != null)
                    {
                        var r = _runtimeOutlineModel.transform.GetChild(0).GetComponent<Renderer>();
                        r.GetPropertyBlock(_propBlock);

                        if (_firstTimeOutlined)
                        {
                            _firstTimeOutlined = false;
                            if (_levelInfo != null)
                            {
                                _propBlock.SetFloat(Contrast, _levelInfo.BlockContrast);
                                _propBlock.SetFloat(Intensity, _levelInfo.ColorIntensity);
                                _propBlock.SetFloat(ShadowStrength, _levelInfo.ShadowStrength);
                                _propBlock.SetColor(ShadowTint, _levelInfo.ShadowTint);
                            }
                        }
                        
                        if (BlockState != BlockStateEnum.Visible)
                        {
                            if (OcularColor != _initialColor)
                                _propBlock.SetColor("_Color", StateToColor(_initialColor));
                            else
                                _propBlock.SetColor("_Color", StateToColor(((ButtonWalkable)Controllers[0]).Color));
                        }
                        else
                        {
                            if (OcularColor != _initialColor)
                                _propBlock.SetColor("_Color", StateToColor(_initialColor));
                            else
                                _propBlock.SetColor("_Color", StateToColor(((ButtonWalkable)Controllers[0]).Color));
                        }
                        r.SetPropertyBlock(_propBlock);

                    }

                    if (transform.HasComponent<Walkable>(out var walkable))
                    {
                        var visible = _blockState == BlockStateEnum.Visible;

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
                        walkable.SetEnabled(visible);
                    }
                    break;
                case ColorableType.ColorChanging:
                    OcularColor = GlassesController.CurrentOcularState;
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
                case OcularState.AB: return new Color(1f, 0.55f, 0f);
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
                return InternalStateToColor(_initialColor);

            Color newCol;

            switch (ocularState)
            {
                case OcularState.Z: return Color.white;
                case OcularState.A: ColorUtility.TryParseHtmlString("#BE5151", out newCol); return newCol;
                case OcularState.AB: ColorUtility.TryParseHtmlString("#FF984C", out newCol); return newCol;
                case OcularState.B: ColorUtility.TryParseHtmlString("#C7C43E", out newCol); return newCol;
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
            var count = _models.Length;
            for (var i = 0; i < count; i++)
            {           
                _models[i].SetActive(state);
            }
        }

        private void Start()
        {
            _initialColor = OcularColor;
        }
        
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                _levelInfo = _entity
                    ? GameObject.Find("GameManager").GetComponent<LevelController>().LevelInfo
                    : transform.parent.parent.GetComponent<LevelInfo>();
            }
            
            var childCount = transform.childCount;
            var temp = new List<GameObject>();
            for (var i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (child.CompareTag("Colorable"))
                   temp.Add(child.gameObject);
            }
            _models = temp.ToArray();

            if (_models.Length != 0)
            {
                var numModels = _models.Length;
                _renderers = new MeshRenderer[numModels];
                for (var i = 0; i < numModels; ++i)
                {
                    _renderers[i] = _models[i].GetComponent<MeshRenderer>();
                }

                if (!_dontUseBlockMat)
                {
                    var count = _renderers.Length;
                    for (var i = 0; i < count; ++i)
                    {
                        _renderers[i].sharedMaterial = _blockMat;
                    }
                }
                
                _propBlock = new MaterialPropertyBlock();
                
                if (_levelInfo != null)
                {
                    var rendererCount = _renderers.Length;
                    for (var i = 0; i < rendererCount; ++i)
                    {
                        var r = _renderers[i];
                        r.GetPropertyBlock(_propBlock);
                        _propBlock.SetFloat(Contrast, _levelInfo.BlockContrast);
                        _propBlock.SetFloat(Intensity, _levelInfo.ColorIntensity);
                        _propBlock.SetFloat(ShadowStrength,
                            transform.HasComponent<GoalAnimationController>()
                                ? _levelInfo.GoalShadowStrength
                                : _levelInfo.ShadowStrength);
                        _propBlock.SetColor(ShadowTint,
                            transform.HasComponent<GoalAnimationController>()
                                ? _levelInfo.GoalShadowTint
                                : _levelInfo.ShadowTint);
                        r.SetPropertyBlock(_propBlock);
                    }
                }
            }
            else
            {
                // This shouldn't really ever happen
                Debug.Log(name + " has no colorable models but is tagged as colorable!");
            }

            OcularColor = _ocularColor;

            _firstTimeOutlined = true;

            GlassesController.OnGlassesToggled += InternalOnGlassesToggled;
        }

        private void OnDisable()
        {
            GlassesController.OnGlassesToggled -= InternalOnGlassesToggled;
        }

        private void Awake()
        {
            if (!Application.isPlaying) return;

            Controllers = new List<IController>();

            if (transform.HasComponent<SlopeWalkable>())
            {
                Addressables.LoadAssetAsync<GameObject>("stair_outline_model").Completed += result =>
                {
                    _outlineModel = result.Result;
                };
            }
            else
            {
                Addressables.LoadAssetAsync<GameObject>("block_outline_model").Completed += result =>
                {
                    _outlineModel = result.Result;
                };
            }
        }

        private void OnDestroy()
        {
            if (Controllers == null) return;
            
            var count = Controllers.Count; 
            for (var i = 0; i < count; i++)
            {
                ((ButtonWalkable) Controllers[i]).TargetBlocks.Remove(this);
            }
        }
    }
}
