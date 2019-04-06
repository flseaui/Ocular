using System;
using System.Data.SqlTypes;
using Level;
using Level.Objects;
using Misc;
using UnityEngine;

namespace UI {
    public class Indicator : MonoBehaviour
    {
        public static Action<Walkable> OnWalkableClicked;

        private readonly int[] _slopeAngles = { 0, 270, 180, 90 };

        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {              
                    SetRotation(walkable, hit);

                    if (Input.GetMouseButtonDown(0))
                        OnWalkableClicked?.Invoke(walkable);
                }
            }
            else
                transform.position = new Vector3(-100, -100, -100);
        }

        private void SetRotation(Walkable walkable, RaycastHit hit, float yShift = .55f)
        {
            switch (walkable)
            {
                case SlopeWalkable _:
                    yShift = 0;  break;
                case ButtonWalkable _:
                    yShift = .11f; break;
            }
            
            transform.localRotation = Quaternion.Euler(walkable.GetType() == typeof(SlopeWalkable) ? 
                new Vector3(45, _slopeAngles[(int) ((SlopeWalkable) walkable).DirectionFacing], 0) : new Vector3(90, 0, 0));
            var hitPos = hit.transform.position;
            transform.position = new Vector3(hitPos.x, hitPos.y + yShift, hitPos.z);
        }        
    }
}