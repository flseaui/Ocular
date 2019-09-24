using Game;
using Level.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpotlightColor : MonoBehaviour
    {
        private Image _spotlight;
        
        private void Awake()
        {
            _spotlight = GetComponent<Image>();
            _spotlight.color = Colorable.StateToColor(GlassesController.CurrentOcularState);
            GlassesController.OnGlassesToggled += () =>
                {
                    _spotlight.color = Colorable.StateToColor(GlassesController.CurrentOcularState);
                };
        }
    }
}