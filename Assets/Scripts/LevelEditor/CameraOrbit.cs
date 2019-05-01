using System;
using UnityEngine;

namespace LevelEditor
{
    public class CameraOrbit : MonoBehaviour
    {
        [NonSerialized] public Transform Target;
        
        [SerializeField] private float _rotateSpeed;

        void LateUpdate()
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