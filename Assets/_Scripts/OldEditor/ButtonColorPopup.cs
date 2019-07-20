using System;
using UnityEngine;

namespace OldEditor
{
    public class ButtonColorPopup : MonoBehaviour
    {
        [NonSerialized]
        public Transform Target;
        
        private void Update()
        {
            var pos = Camera.main.WorldToScreenPoint(Target.position);
            transform.position = new Vector3(pos.x, pos.y + 90, pos.z);
        }
    }
}