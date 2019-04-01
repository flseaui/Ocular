using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[ExecuteInEditMode]
public class Colorable : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Color _color;
    private MaterialPropertyBlock _propBlock;
    private Renderer[] _renderers;
    private LevelInfo _levelInfo;
    
    private GameObject[] _models;

    private void OnEnable()
    {
        var temp = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Colorable"))
                temp.Add(child.gameObject);
        }
        _models = temp.ToArray();
        _renderers = _models.Select(m => m.GetComponent<Renderer>()).ToArray();
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
        
        GlassesController.OnGlassesToggled += color =>
        {
            if (Color == Color.white) return;
            
            var visibile =
                _levelInfo.BlockColors.FirstOrDefault(x => x.Color == Color)?.Requirements.Contains(color) == true;

            if (_models[0].activeSelf == visibile) return;
            
            if (transform.HasComponent<Walkable>(out var walkable))
            {
                walkable.CheckBelow(!visibile);
                walkable.Enabled = visibile;
            }

            SetModelsState(visibile);

        };
    }

    private void SetModelsState(bool state)
    {
        _models.ForEach(m => m.SetActive(state));
    }
    
    [ColorPalette("RGB")]
    [ShowInInspector]
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
}
