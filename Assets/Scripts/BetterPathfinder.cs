using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Priority_Queue;
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
        float Heuristic(Vector3 a, Vector3 b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        float MovementCost(Node a, Node b)
        {
            return 1;
        }
        
        var goalNode = destination.Node;
        var path = new List<Walkable>();
        var frontier = new FastPriorityQueue<Node>(MapGenerator.NUM_WALKABLES);
        var cameFrom = new Dictionary<int, Node>();
        var costSoFar = new Dictionary<int, float>();
        costSoFar.Add(start.UniqueId, 0);
        frontier.Enqueue(start.Node, 0);
        
        Node current;
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goalNode)
                break;
            
            foreach (var neighbor in current.Neighbors.Where(x => x.Enabled))
            {
                var newCost = costSoFar[current.Walkable.UniqueId] + MovementCost(current, neighbor);
                
                if (!costSoFar.ContainsKey(neighbor.Walkable.UniqueId) || newCost < costSoFar[neighbor.Walkable.UniqueId])
                {
                    costSoFar[neighbor.Walkable.UniqueId] = newCost;
                    frontier.Enqueue(neighbor, newCost + Heuristic(goalNode.Walkable.transform.localPosition, neighbor.Walkable.transform.localPosition));
                    cameFrom.Add(neighbor.Walkable.UniqueId, current);
                }
            }
        }
        
        // retrace path
        current = goalNode;
        while (current.Walkable.UniqueId != start.UniqueId)
        {
            path.Add(current.Walkable);
            current = cameFrom[current.Walkable.UniqueId];
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
