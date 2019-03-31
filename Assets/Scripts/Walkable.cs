using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Walkable : MonoBehaviour, IEquatable<Walkable>
{
    public List<Walkable> Neighbors;

    public bool Enabled;

    [ShowInInspector]
    [ReadOnly]
    public int UniqueId { get; private set; }
    
    public void Awake()
    {
        Enabled = true;
        UniqueId = GetInstanceID();
    }

    public void AddNeighbor(Walkable neighbor)
    {
        if (neighbor != this)
            Neighbors.Add(neighbor);
    }
    
    public virtual void CheckForNeighbors()
    {
        // Up
        if (Physics.Raycast(transform.localPosition, new Vector3(0, 1, 0), out var hit, 1))
        {
            if (hit.transform.ParentHasComponent<Walkable>())
            {
                Enabled = false;
            }
        }
        
        // Left Right Forward Back
        for (var x = -1; x < 2; x++)
        {
            for (var z = -1; z < 2; z++)
            {
                if (Math.Abs(x) == Math.Abs(z)) continue;     
                if (!Physics.Raycast(transform.localPosition, new Vector3(x, 0, z), out hit, 1)) continue;
                
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {
                    AddNeighbor(walkable);
                }
            }
        }
    }

    public void CheckBelow(bool state)
    {
        if (Physics.Raycast(new Vector3(transform.localPosition.x, transform.localPosition.y - .1f, transform.localPosition.z), new Vector3(0, -1, 0), out var hit, 1))
        {
            if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                walkable.Enabled = state;
            }
        }
    }

    public bool Equals(Walkable other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && UniqueId == other.UniqueId;
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

#if  UNITY_EDITOR
    protected void OnDrawGizmos()
    {        
        if (!Enabled || Neighbors == null)
            return;
        Gizmos.color = Color.black;
        foreach (var neighbor in Neighbors)
        {
            if (!neighbor.Enabled) continue;
            if (neighbor != null)
                Gizmos.DrawLine(transform.position + new Vector3(0, 1.5f, 0), neighbor.transform.position + new Vector3(0, 1.5f, 0));
        }
    }
#endif
}
