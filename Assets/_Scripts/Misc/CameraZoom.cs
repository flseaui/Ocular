using Level;
using UI;
using UnityEngine;

namespace Misc
{
    public class CameraZoom : MonoBehaviour
    {

        private Camera _camera;
        private Vector3 _pos;
        private Vector3 _cameraPos;
        private float _cameraSize;
        public float constantx = .5f;
        public float constanty = .3f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Level.LevelController.OnLevelLoaded += LevelSetUp;
        }

        private void Update()
        {
            if (PauseMenu.GameIsPaused) return;

            var scrollDelta = Input.mouseScrollDelta.y;
            scrollDelta = .3f * -scrollDelta;
            if (scrollDelta != 0)
            {
                if (scrollDelta > 0)
                {
                    if (_camera.orthographicSize + scrollDelta < 8)
                    {
                        _camera.orthographicSize += scrollDelta;
                    }
                    else
                    {
                        _camera.orthographicSize = 8;
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
                if (_camera.orthographicSize < _cameraSize) {
                    var angle = _camera.transform.eulerAngles.y + 45;
                    var zoom = (_cameraSize - _camera.orthographicSize) / (_cameraSize - 2);
                    var levelx = (Mathf.Cos((angle * Mathf.PI) / 180) * (_pos.x + _pos.z)) - (Mathf.Sin((angle * Mathf.PI) / 180) * (_pos.z - _pos.x));
                    var levely = (Mathf.Sin((angle * Mathf.PI) / 180) * (_pos.x + _pos.z)) + (Mathf.Cos((angle * Mathf.PI) / 180) * (_pos.z - _pos.x));
                    Debug.Log("Angle: " + angle + " Level x: " + levelx + " Level y: " + levely);
                    _camera.transform.localPosition = new Vector3((levelx * zoom * constantx) + _cameraPos.x, (levely * zoom * constanty) + _cameraPos.y, _cameraPos.z);
                }
                else
                {
                    _camera.transform.localPosition = _cameraPos;
                }
            }
        }

        private void LevelSetUp()
        {
            var levelcon = GameObject.Find("GameManager").GetComponent<Level.LevelController>().CurrentLevelInfo;
            _pos = levelcon.transform.Find("MainFloor").transform.position;
            _cameraPos = levelcon.CameraPosition;
            _cameraSize = levelcon.CameraSize;
        }

        private void OnDestroy()
        {
            Level.LevelController.OnLevelLoaded -= LevelSetUp;
        }

    }
}
