using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class Colorable : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Color _color;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    private LevelInfo _levelInfo;
    
    private GameObject _model;

    private void OnEnable()
    {
        _model = transform.Find("Model").gameObject;
        _renderer = _model.GetComponent<Renderer>();
        _levelInfo = transform.parent.parent.GetComponent<LevelInfo>();
        _propBlock = new MaterialPropertyBlock();
        Color = Color;
        _model.SetActive(true);
    }

    public void Initialize()
    {
        if (Color == Color.white) return;
        _model.SetActive(false);
        if (transform.HasComponent<Walkable>(out var walkable))
        {
            walkable.CheckBelow(walkable.Enabled);
            walkable.Enabled = false;
        }
    }
    
    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        GlassesController.OnGlassesToggled += color =>
        {
            if (Color == Color.white) return;
            
            var visibile =
                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements.Contains(color) == true;

            if (_model.activeSelf == visibile) return;
            
            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(!visibile);
                walkable.Enabled = visibile;
            }
        
            _model.SetActive(visibile);

        };
    }
    
    [ColorPalette("RGB")]
    [ShowInInspector]
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", value);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
