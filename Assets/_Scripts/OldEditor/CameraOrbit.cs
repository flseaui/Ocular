#if UNITY_EDITOR

using System;
using UnityEngine;

namespace OldEditor
{
    public class CameraOrbit : MonoBehaviour
    {
        [NonSerialized] public Transform Target;
        
        [SerializeField] private float _rotateSpeed;

        private bool _canRotate;
        
        private void Awake()
        {
            LevelEditor.OnLevelPlayToggle += ToggleControl;
        }

        private void OnDestroy()
        {
            LevelEditor.OnLevelPlayToggle -= ToggleControl;
        }

        private void Start()
        {
            _canRotate = true;
        }
        
        private void ToggleControl(bool state)
        {
            if (state)
            {
                transform.localRotation = Quaternion.Euler(Vector3.zero);
                transform.localPosition = new Vector3(0, 0, -10);
            }

            _canRotate = !state;
        }
        
        void LateUpdate()
        {
            if (_canRotate)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    transform.RotateAround(Target.position, Vector3.up, _rotateSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    transform.RotateAround(Target.position, -Vector3.up, _rotateSpeed * Time.deltaTime);
                }
            }
        }
    }
}

#endif