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

        private void Awake()
        {
            _levelController = GameObject.Find("GameManager").GetComponent<LevelController>();
        }
        
        private void Start()
        {
            DOTween.Init();
        }

        private void Update()
        {            
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
            
            if(Turning) return;
            
            var left = Input.GetKey(KeyCode.LeftArrow);
            var right = Input.GetKey(KeyCode.RightArrow);

            if (left && right) return;

            TweenerCore<Quaternion, Vector3, QuaternionOptions> rotate;
            
            if (left || _bufferLeft)
            {
                Turning = true;
                _bufferLeft = false;
                rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 90, 0),
                    .3f);
                rotate.onComplete += () => Turning = false;
            }

            if (!right && !_bufferRight) return;
            
            Turning = true;
            _bufferRight = false;
            rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, -90, 0),
                .3f);
            rotate.onComplete += () => Turning = false;
            

        }
    }
}