using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game;
using Level;
using Player;
using UnityEngine;

namespace Misc
{
    public class CameraController : MonoBehaviour
    {
        public static bool Turning;
        private bool _bufferLeft;
        private bool _bufferRight;

        private LevelController _levelController;

        [SerializeField] private bool _isInEditor;

        private CameraZoom _zoomer;

        private LevelInfo LevelInfo
        {
            get
            {
#if UNITY_EDITOR
                return _isInEditor
                    ? GameObject.Find("LevelEditor").GetComponent<LevelEditor.LevelEditor>().EditorLevelInfo
                    : _levelController.CurrentLevelInfo;
#else
                return _levelController.CurrentLevelInfo;
#endif
            }
        }

        private void Awake()
        {
            if (!_isInEditor)
                _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
            
            _zoomer = transform.GetChild(0).GetComponent<CameraZoom>();
            LevelController.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            LevelController.Falling = true;
            var lr = transform.rotation.eulerAngles;
            switch (LevelInfo.CameraStartDirection)
            {
                case Direction.Forward:
                    transform.rotation = Quaternion.Euler(lr.x, 45, lr.z);
                    break;
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(lr.x, 135, lr.z);
                    break;
                case Direction.Back:
                    transform.rotation = Quaternion.Euler(lr.x, -135, lr.z);
                    break;
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(lr.x, -45, lr.z);
                    break;
            }
        }

        private void Start()
        {
            DOTween.Init();
        }

        public void TurnLeft()
        {
            if (Turning || LevelController.Falling || Pathfinder.Navigating) return;
            TweenerCore<Quaternion, Vector3, QuaternionOptions> rotate;
            Turning = true;
            _zoomer.RecalcZoom(transform.eulerAngles.y + 135, true);
            rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 90, 0),
                .3f);
            rotate.onComplete += () =>
            {
                Turning = false;
            };
        }

        public void TurnRight()
        {
            if (Turning || LevelController.Falling || Pathfinder.Navigating) return;
            TweenerCore<Quaternion, Vector3, QuaternionOptions> rotate;
            Turning = true;
            _zoomer.RecalcZoom(transform.eulerAngles.y - 45, true);
            rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, -90, 0),
                .3f);
            rotate.onComplete += () =>
            {
                Turning = false;
            };
        }

        private void Update()
        {
            // TODO janky method to check if in editor and not testing
            if (!GameObject.Find("GameManager"))
                return;
            
            if (Turning && Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                _bufferLeft = true;
                _bufferRight = false;
            }

            if (Turning && Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                _bufferRight = true;
                _bufferLeft = false;
            }

            if (Turning) return;

            var left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            var right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

            if (left && right) return;

            if (left || _bufferLeft)
            {
                TurnLeft();
                _bufferLeft = false;
            }

            if (!right && !_bufferRight) return;

            TurnRight();
            _bufferRight = false;


        }
    }
}