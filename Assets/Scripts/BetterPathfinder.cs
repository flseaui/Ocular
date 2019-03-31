using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class BetterPathfinder : MonoBehaviour
{
    private Walkable CurrentPosition => GetCurrentWalkable();
    private Walkable _queuedDestination;
    private Walkable _currentDesition;
    private List<Walkable> _currentPath;
    public float WalkSpeed = 5.0f;
    public bool Navigating;

    private void Awake()
    {
        Indicator.OnWalkableClicked += NavigateTo;
    }

    private void NavigateTo(Walkable destination)
    {
        _currentPath = GeneratePath(GetCurrentWalkable(), destination);
    }
    
    private List<Walkable> GeneratePath(Walkable start, Walkable destination)
    {
        var path = new List<Walkable>();
        var frontier = new Queue<Walkable>();
        var cameFrom = new Dictionary<int, Walkable>();
        var pathFinished = false;

        frontier.Enqueue(start);
        
        Walkable current;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == destination)
                break;
            
            foreach (var neighbor in current.Neighbors)
            {
                if (!neighbor.Enabled)
                    continue;
                
                if (!cameFrom.ContainsKey(neighbor.UniqueId))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom.Add(neighbor.UniqueId, current);
                }
            }
        }
        
        // retrace path
        current = destination;
        while (current.UniqueId != start.UniqueId)
        {
            path.Add(current);
            current = cameFrom[current.UniqueId];
        }

        path.Reverse();
        return path;
    }

    private Walkable GetCurrentWalkable()
    {
        return Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out var hit, 1) 
        ? hit.transform.parent.GetComponent<Walkable>() : null;
    }
    
    void Update()
    {
        if (Navigating)
        {
            
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_currentPath != null)
        {
            foreach (var walkable in _currentPath)
            {
                Gizmos.DrawCube(walkable.transform.position + Vector3.up / 2, new Vector3(.5f, .5f, .5f));
            }
        }
    }
#endif
}
