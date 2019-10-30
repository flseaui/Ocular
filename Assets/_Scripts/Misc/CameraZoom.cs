using UnityEngine;

namespace Misc
{
    public class CameraZoom : MonoBehaviour
    {

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            var scrollDelta = Input.mouseScrollDelta.y;
            scrollDelta = .3f * -scrollDelta;
            if (scrollDelta > 0)
            {
                if (_camera.orthographicSize + scrollDelta < 10)
                {
                    _camera.orthographicSize += scrollDelta;
                }
                else
                {
                    _camera.orthographicSize = 10;
                }
            }
            else
            {
                if (_camera.orthographicSize + scrollDelta > 2)
                {
                    _camera.orthographicSize += scrollDelta;
                }
                else
                {
                    _camera.orthographicSize = 2;
                }
            }
        }

    }
}
