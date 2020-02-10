using System;
using System.Collections.Generic;
using System.Linq;
using Misc;
using Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Level.Objects {
    [SelectionBase]
    public class Walkable : MonoBehaviour, IEquatable<Walkable>
    {
        public Node Node;

        [ShowInInspector]
        public bool Enabled
        {
            get => Node != null && Node.Enabled;
            set
            {
                if (Node != null)
                    Node.Enabled = value;
            }
        }

        [ShowInInspector, ReadOnly]
        public int UniqueId { get; private set; }

        public bool JustChangedState;
        
        
#if UNITY_EDITOR
        [ShowInInspector, HideInEditorMode]
        private List<Walkable> Neighbors =>
            Application.isPlaying ? Node.Neighbors.Select(n => n.Walkable).ToList() : new List<Walkable>();
#endif

        public void SetEnabled(bool state)
        {
            if (JustChangedState)
            {
                JustChangedState = false;
                return;
            }

            Enabled = state;
        }
        
        public bool Equals(Walkable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && UniqueId == other.UniqueId;
        }

        public void Awake()
        {
            Node = new Node(this);
            UniqueId = GetInstanceID();
            OnAwake();
        }

        protected virtual void OnAwake() { }

        public void AddNeighbor(Walkable neighbor)
        {
            if (neighbor != this)
                Node.Neighbors.Add(neighbor.Node);
        }

        public virtual void CheckForNeighbors()
        {
            // Up
            if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out var vHit, 1))
            {
                if (vHit.transform.ParentHasComponent<Walkable>())
                    Enabled = false;
            }

            // Left Right Forward Back
            for (var x = -1; x < 2; x++)
            {
                for (var z = -1; z < 2; z++)
                {
                    if (Math.Abs(x) == Math.Abs(z)) continue;
                    var ray = new Ray(transform.position, new Vector3(x, 0, z));
                    if (!Physics.Raycast(ray, out var hHit, 1, LayerMask.GetMask("Model"))) continue;

                    if (hHit.transform.ParentHasComponent<Walkable>(out var walkable)) AddNeighbor(walkable);
                }
            }
        }

        public void CheckBelow(bool state)
        {
            if (Physics.Raycast(
                new Vector3(transform.position.x, transform.position.y - .1f, transform.position.z),
                new Vector3(0, -1, 0), out var hit, 3, LayerMask.GetMask("Model")))
            {
                if (hit.collider is null) return;
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {
                    walkable.JustChangedState = true;
                    walkable.Enabled = state;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Walkable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ UniqueId.GetHashCode();
            }
        }

        public static bool operator ==(Walkable left, Walkable right) => Equals(left, right);

        public static bool operator !=(Walkable left, Walkable right) => !Equals(left, right);

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (!Enabled || Node.Neighbors == null)
                return;
            
            Gizmos.color = Color.green;
            foreach (var neighbor in Node.Neighbors)
            {
                if (neighbor is null) continue;
                
                Gizmos.DrawCube(neighbor.Walkable.transform.position, new Vector3(.5f, .5f, .5f));
            }
        }

        protected void OnDrawGizmos()
        {
            if (!Enabled || Node.Neighbors == null)
                return;
            Gizmos.color = Color.black;
            foreach (var neighbor in Node.Neighbors)
            {
                if (neighbor is null) continue;
                if (!neighbor.Walkable.Enabled) continue;
                Gizmos.DrawLine(transform.position + new Vector3(0, 1.5f, 0),
                    neighbor.Walkable.transform.position + new Vector3(0, 1.5f, 0));
            }
        }
#endif
    }
}