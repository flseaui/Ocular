using System;
using Unity.Mathematics;
using UnityEngine;

namespace LevelEditor
{
    public class ButtonColorPopup : MonoBehaviour
    {
        [NonSerialized]
        public Transform Target;
        
        private void Update()
        {
            var pos = Camera.main.WorldToScreenPoint(Target.position);
            transform.position = new float3(pos.x, pos.y + 90, pos.z);
        }
    }
}