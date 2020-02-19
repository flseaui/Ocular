using System;
using DG.Tweening;
using Level;
using Sirenix.OdinInspector;
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
        private float _projectedSize;
        [NonSerialized, ShowInInspector]
        public float ConstantX = .6f;
        [NonSerialized, ShowInInspector]
        public float ConstantY = .3f;

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
                        DOTween.To(() => _camera.orthographicSize, x => _camera.orthographicSize = x,
                            _camera.orthographicSize + scrollDelta, .1f);
                        _projectedSize = _camera.orthographicSize + scrollDelta;
                    }
                    else
                    {
                        _camera.orthographicSize = 8;
                        _projectedSize = 8;
                    }
                }
                else
                {
                    if (_camera.orthographicSize + scrollDelta > 2)
                    {
                        DOTween.To(() => _camera.orthographicSize, x => _camera.orthographicSize = x,
                            _camera.orthographicSize + scrollDelta, .1f);
                        _projectedSize = _camera.orthographicSize + scrollDelta;
                    }
                    else
                    {
                        _camera.orthographicSize = 2;
                        _projectedSize = 2;
                    }
                }
                RecalcZoom(_camera.transform.eulerAngles.y + 45);
            }
        }

        public void RecalcZoom(float angle)
        {
            if (_projectedSize < _cameraSize) {
                var zoom = (_cameraSize - _projectedSize) / (_cameraSize - 2);
                var levelx = (Mathf.Cos((angle * Mathf.PI) / 180) * (_pos.x + _pos.z)) - (Mathf.Sin((angle * Mathf.PI) / 180) * (_pos.z - _pos.x));
                var levely = (Mathf.Sin((angle * Mathf.PI) / 180) * (_pos.x + _pos.z)) + (Mathf.Cos((angle * Mathf.PI) / 180) * (_pos.z - _pos.x));
                Debug.Log(zoom);
                    _camera.transform.DOLocalMove(new Vector3((levelx * zoom * ConstantX) + _cameraPos.x,
                    (levely * zoom * ConstantY) + _cameraPos.y, _cameraPos.z), .1f);
            }
            else
            {
                    _camera.transform.DOLocalMove(_cameraPos, .1f);
            }
        }
        
        private void LevelSetUp()
        {
            var levelcon = GameObject.Find("GameManager").GetComponent<Level.LevelController>().CurrentLevelInfo;
            _pos = levelcon.transform.Find("MainFloor").transform.position;
            _cameraPos = levelcon.CameraPosition;
            _cameraSize = levelcon.CameraSize;
            _projectedSize = _camera.orthographicSize;
            if (levelcon.HasCustomConstants)
            {
                ConstantX = levelcon.ZoomConstantX;
                ConstantY = levelcon.ZoomConstantY;
            }
            else
            {
                ConstantX = .5f;
                ConstantY = .3f;
            }
        }

        private void OnDestroy()
        {
            Level.LevelController.OnLevelLoaded -= LevelSetUp;
        }

    }
}
