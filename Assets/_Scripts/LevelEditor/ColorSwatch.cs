using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorSwatch : MonoBehaviour, IPointerDownHandler 
{
    private ColorPalette _palette;
    [SerializeField] private Sprite _selectedSprite;
    [OnValueChanged(nameof(ColorChanged)), ColorPalette]
    public Color Color;

    private void ColorChanged()
    {
        GetComponent<Image>().color = Color;
    }
    
    public void Deselect()
    {
        GetComponent<Image>().sprite = null;
    }
    
    private void Awake()
    {
        _palette = GetComponentInParent<ColorPalette>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        GetComponent<Image>().sprite = _selectedSprite;
        _palette.SetColor(Color);
    }
    
}
