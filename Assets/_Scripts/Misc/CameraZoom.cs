using System;
using DG.Tweening;
using Level;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            LevelController.OnLevelLoaded += LevelSetUp;
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name.Equals("NewEditor"))
            {
                LevelSetUp(true);
            }
        }

        private void LevelSetUp()
        {
            LevelSetUp(false);
        }

        private void Update()
        {
            if (PauseMenu.GameIsPaused) return;

            var scrollDelta = Input.mouseScrollDelta.y;
            scrollDelta = Math.Min(.3f * -scrollDelta, 1);
            
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
                        DOTween.To(() => _camera.orthographicSize, x => _camera.orthographicSize = x,
                            8, .1f);
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
                        DOTween.To(() => _camera.orthographicSize, x => _camera.orthographicSize = x,
                            2, .1f);
                        _projectedSize = 2;
                    }
                }
                RecalcZoom(_camera.transform.eulerAngles.y + 45, true);
            }
        }

        public void RecalcZoom(float angle, bool smooth)
        {
            if (_projectedSize < _cameraSize)
            {
                var zoom = (_cameraSize - _projectedSize) / (_cameraSize - 2);
                var levelX = Mathf.Cos(angle * Mathf.PI / 180) * (_pos.x + _pos.z) - Mathf.Sin(angle * Mathf.PI / 180) * (_pos.z - _pos.x);
                var levelY = Mathf.Sin(angle * Mathf.PI / 180) * (_pos.x + _pos.z) + Mathf.Cos(angle * Mathf.PI / 180) * (_pos.z - _pos.x);
                
                Debug.Log(zoom);
                
                if (smooth)
                    _camera.transform.DOLocalMove(new Vector3(levelX * zoom * ConstantX + _cameraPos.x,
                        levelY * zoom * ConstantY + _cameraPos.y, _cameraPos.z), .1f);
                else
                    _camera.transform.localPosition = new Vector3(levelX * zoom * ConstantX + _cameraPos.x,
                        levelY * zoom * ConstantY + _cameraPos.y, _cameraPos.z);
            }
            else
            {
                if (smooth)
                    _camera.transform.DOLocalMove(_cameraPos, .01f);
                else
                    _camera.transform.localPosition = _cameraPos;
            }
        }
        
        private void LevelSetUp(bool inEditor)
        {
#if UNITY_EDITOR
            var levelInfo = inEditor
                ? GameObject.Find("LevelEditor").GetComponent<LevelEditor.LevelEditor>().EditorLevelInfo
                : GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo;
#else
            var levelInfo = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo;
#endif

            _pos = levelInfo.transform.Find("MainFloor").transform.position;
            _cameraPos = levelInfo.CameraPosition;
            _cameraSize = levelInfo.CameraSize;
            _projectedSize = _camera.orthographicSize;
            if (levelInfo.HasCustomConstants)
            {
                ConstantX = levelInfo.ZoomConstantX;
                ConstantY = levelInfo.ZoomConstantY;
            }
            else
            {
                ConstantX = .5f;
                ConstantY = .3f;
            }
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= LevelSetUp;
        }

    }
}
