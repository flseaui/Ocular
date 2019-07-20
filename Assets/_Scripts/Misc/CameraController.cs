using DG.Tweening;
using UnityEngine;

namespace Misc
{
    public class CameraController : MonoBehaviour
    {
        private void Start()
        {
            DOTween.Init();
        }

        private bool _moving;

        private void Update()
        {
            if (_moving) return;

            var left = Input.GetKey(KeyCode.LeftArrow);
            var right = Input.GetKey(KeyCode.RightArrow);

            var angle = left ? right ? 0 : -90 : right ? 90 : 0;
            
            _moving = true;
            var rotate = transform.DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, angle, 0), .2f);
            rotate.onComplete += () => _moving = false;
        }
    }
}