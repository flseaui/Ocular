using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Level.Objects.Clones
{
    public class DirectionalCloneSpawn : MonoBehaviour
    {
        public enum Cardinal
        {
            North,
            East,
            South,
            West,
            Count
        }
        
        [SerializeField, HideInInspector] 
        private Cardinal _direction;

        [ShowInInspector]
        public Cardinal Direction
        {
            get => _direction;
            set
            {
                if (value == Cardinal.Count)
                {
                    Debug.Log("Cant set Direction to Cardinal.Count");
                    return;
                }
                
                ChangeFacing(value);
                
                _direction = value;
            }
        }

        public void IncrementDirection()
        {
            var index = (int) Direction + 1;
            if (index > (int) Cardinal.Count - 1)
                index = 0;
            Direction = (Cardinal) index;
        }
        
        public void DecrementDirection()
        {
            var index = (int) Direction - 1;
            if (index < 0)
                index = (int) Cardinal.Count - 1;
            Direction = (Cardinal) index;
        }
        
        private void ChangeFacing(Cardinal newDirection)
        {
            var rot = transform.eulerAngles;
            switch (newDirection)
            {
                case Cardinal.North:
                    transform.eulerAngles = new float3(
                        rot.x,
                        270,
                        rot.z
                    );
                    break;
                case Cardinal.East:
                    transform.eulerAngles = new float3(
                        rot.x,
                        0,
                        rot.z
                    );
                    break;
                case Cardinal.South:
                    transform.eulerAngles = new float3(
                        rot.x,
                        90,
                        rot.z
                    );
                    break;
                case Cardinal.West:
                    transform.eulerAngles = new float3(
                        rot.x,
                        180,
                        rot.z
                    );
                    break;
                case Cardinal.Count:
                    break;
                default:
                    return;
            }
        }
    }
}