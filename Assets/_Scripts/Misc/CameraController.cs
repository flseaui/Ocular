using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game;
using Level;
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
        
        private void Awake()
        {
            if (_isInEditor) return;
            
            _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
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
            switch (_levelController.CurrentLevelInfo.CameraStartDirection)
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
            if (Turning || LevelController.Falling) return;
            TweenerCore<Quaternion, Vector3, QuaternionOptions> rotate;
            Turning = true;
            rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 90, 0),
                .3f);
            rotate.onComplete += () => Turning = false;
        }

        public void TurnRight()
        {
            if (Turning || LevelController.Falling) return;
            TweenerCore<Quaternion, Vector3, QuaternionOptions> rotate;
            Turning = true;
            rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, -90, 0),
                .3f);
            rotate.onComplete += () => Turning = false;
        }

        private void Update()
        {
            if (!GameObject.Find("GameManager"))
                return;
            
            if (Turning && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _bufferLeft = true;
                _bufferRight = false;
            }

            if (Turning && Input.GetKeyDown(KeyCode.RightArrow))
            {
                _bufferRight = true;
                _bufferLeft = false;
            }

            if (Turning) return;

            var left = Input.GetKey(KeyCode.LeftArrow);
            var right = Input.GetKey(KeyCode.RightArrow);

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