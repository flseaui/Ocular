using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace OldEditor
{
    public class PlayButton : MonoBehaviour
    {
        [SerializeField] private Sprite _playButtonSprite;
        [SerializeField] private Sprite _pauseButtonSprite;

        [ShowInInspector, ReadOnly] private bool _state;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Toggle()
        {
            _state = !_state;
            _image.sprite = _state ? _pauseButtonSprite : _playButtonSprite;
        }
    }
}