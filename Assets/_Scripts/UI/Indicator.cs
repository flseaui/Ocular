using System;
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

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Model")))
            {
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable) && walkable.Enabled)
                {
                    switch (walkable)
                    {
                        case SlopeWalkable slope:
                           SetTransform(.26f, () => transform.localRotation = Quaternion.Euler(45, _slopeAngles[(int) slope.DirectionFacing], 0));
                            break;
                        case ButtonWalkable _:
                            SetTransform(0.61f);
                            break;
                        default:
                            SetTransform(0.55f);
                            break;
                    }

                    if (Input.GetMouseButtonDown(0))
                        OnWalkableClicked?.Invoke(walkable);
                }
            }
            else
            {
                transform.position = new Vector3(-1000, -1000, -1000);
            }
            
            void SetTransform(float yShift, Action rotation = null)
            {
                if (rotation is null)
                    transform.localRotation = Quaternion.Euler(90, 0, 0);
                else
                    rotation();
                
                var hitPos = hit.transform.position;
                transform.position = new Vector3(hitPos.x, hitPos.y + yShift, hitPos.z);
            }
        }
    }
}