#if UNITY_EDITOR

using Level.Objects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OcularState = Game.GlassesController.OcularState;

namespace LevelEditor
{
    public class ColorSwatch : MonoBehaviour, IPointerDownHandler 
    {
        private ColorPalette _palette;
        [SerializeField] private Sprite _selectedSprite;
        [OnValueChanged(nameof(ColorChanged))]
        public OcularState Color;

        private Sprite _initialSprite;

        private Image _image;
        
        private void ColorChanged()
        {
            _image.color = Colorable.StateToColor(Color);
        }
    
        public void Deselect()
        {
            _image.sprite = _initialSprite;
        }
    
        private void Awake()
        {
            _palette = GetComponentInParent<ColorPalette>();
            _image = GetComponent<Image>();
            _initialSprite = _image.sprite;
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
        }

        public void Select()
        {
            _image.sprite = _selectedSprite;
            _palette.SetColor(Color);
        }
    
    }
}

#endif
