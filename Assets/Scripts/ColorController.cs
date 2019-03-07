using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[ExecuteInEditMode]
public class ColorController : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Color _color;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;

    private void OnEnable()
    {
        _renderer = transform.Find("Model").GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        Color = Color;
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
