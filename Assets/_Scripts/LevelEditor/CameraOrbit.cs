#if UNITY_EDITOR

using System;
using Misc;
using UnityEngine;

namespace LevelEditor
{
    public class CameraOrbit : MonoBehaviour
    {
        private CameraZoom _zoomer;

        [SerializeField] private float _rotateSpeed;

        private bool _canRotate;
        
        private void Awake()
        {
            _zoomer = transform.GetChild(0).GetComponent<CameraZoom>();
            
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
                    var newRot = transform.localRotation.eulerAngles + new Vector3(0, _rotateSpeed * Time.deltaTime, 0);
                    _zoomer.RecalcZoom(newRot.y + 45, false);
                    transform.localRotation = Quaternion.Euler(newRot);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    var newRot = transform.localRotation.eulerAngles - new Vector3(0, _rotateSpeed * Time.deltaTime, 0);
                    _zoomer.RecalcZoom(newRot.y + 45, false);
                    transform.localRotation = Quaternion.Euler(newRot);
                }
            }
        }
    }
}

#endif