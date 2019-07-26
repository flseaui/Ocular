#if UNITY_EDITOR

using Game;
using Level.Objects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OcularState = Game.GlassesController.OcularState;

namespace OldEditor
{
    public class ColorSwatch : MonoBehaviour, IPointerDownHandler 
    {
        private ColorPalette _palette;
        [SerializeField] private Sprite _selectedSprite;
        [OnValueChanged(nameof(ColorChanged))]
        public OcularState Color;

        private void ColorChanged()
        {
            GetComponent<Image>().color = Colorable.StateToColor(Color);
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
}

#endif
