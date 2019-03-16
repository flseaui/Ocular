using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : IEquatable<Waypoint>
{
    public Walkable Walkable;
    public int Counter;

    public bool Equals(Waypoint other)
    {
        return Equals(Walkable, other.Walkable);
    }
}

public class Pathfinder : MonoBehaviour
{
    private Walkable _destination;
    
    private Walkable _currentPosition;
    private Walkable _currentDesition;
    private Queue<Walkable> _currentPath;

    private bool _currentlyMoving;

    private void Awake()
    {
        Indicator.OnWalkableClicked += walkable => _destination = walkable;
    }
    
    private Queue<Walkable> GeneratePath(Walkable destination)
    {
        var path = new Queue<Walkable>();
        var queuedWaypoints = new Queue<Waypoint>();
        var tempQueue = new Queue<Waypoint>();
                    
        var pathFinished = false;
        var counter = 0;
        
        var last = destination;
        
        queuedWaypoints.Enqueue(new Waypoint {Walkable = destination, Counter = 0});
        
        while (!pathFinished)
        {
            ++counter;
            foreach (var neighbor in queuedWaypoints.Dequeue().Walkable.Neighbors)
            {
                if (neighbor.Equals(_currentPosition))
                {
                    queuedWaypoints.Enqueue(new Waypoint {Walkable = neighbor, Counter = counter});
                    path.Enqueue(neighbor);
                    last = neighbor;
                    pathFinished = true;
                    break;
                }
        
                if (ReferenceEquals(queuedWaypoints.FirstOrDefault(x => x.Walkable == neighbor), null))
                    tempQueue.Enqueue(new Waypoint {Walkable = neighbor, Counter = counter});
            }          

            var size = tempQueue.Count;
            for (var i = 0; i < size; i++)
            {
                queuedWaypoints.Enqueue(tempQueue.Dequeue());
            }
        }

        for (var i = counter; i == 0; i--)
        {
            foreach (var neighbor in last.Neighbors)
            {
                if (queuedWaypoints.FirstOrDefault(x => x.Walkable == neighbor).Counter < counter)
                {
                    path.Enqueue(neighbor);
                    last = neighbor;
                    break;
                }
            }          
        }
        return path;
    }

    private Walkable GetCurrentWalkable()
    {
        if (Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out var hit, 1))
            return hit.transform.parent.GetComponent<Walkable>();
        return null;
    }

    private void CheckForNewDestination()
    {
        if (!ReferenceEquals(_currentDesition, _destination))
        {
            _currentDesition = _destination;
            _currentPath = GeneratePath(_destination);
        }
    }
    
   
    void Update()
    {
        _currentPosition = GetCurrentWalkable();

        if (_currentlyMoving)
        {
            //Lerp player, and if at waypoint check for new desitination
        }
        else      
            CheckForNewDestination();
    }
}
