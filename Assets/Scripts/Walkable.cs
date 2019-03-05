using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    public List<Walkable> Neighbors;

    public bool Enabled;

    public void Awake()
    {
        Enabled = true;
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
        for (var x = -1; x < 1; x++)
        {
            for (var z = -1; z < 1; z++)
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
