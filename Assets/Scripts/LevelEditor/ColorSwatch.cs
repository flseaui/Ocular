using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorSwatch : MonoBehaviour, IPointerDownHandler 
{
    private ColorPallette _pallette;
    private Sprite _selectedSprite;
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
        _pallette = GetComponentInParent<ColorPallette>();
        Addressables.LoadAsset<Sprite>("color_swatch_selected").Completed += handle => _selectedSprite = handle.Result;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = _selectedSprite;
        _pallette.SetColor(Color);
    }
}
