using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : IEquatable<Waypoint>
{
    public Walkable walkable;
    public int counter;

    public bool Equals(Waypoint other)
    {
        return Equals(walkable, other.walkable);
    }
}

public class Pathfinder : MonoBehaviour
{ 
    public Walkable SetDestination { get; set; }
    
    private Walkable _currentPosition;
    private Walkable _currentDesition;
    private Queue<Walkable> _currentPath;

    private bool _currentlyMoving;
    
    private Queue<Walkable> GeneratePath(Walkable destination)
    {
        Queue<Walkable> path = new Queue<Walkable>();
        Queue<Waypoint> queuedWaypoints = new Queue<Waypoint>();
        Queue<Waypoint> tempQueue = new Queue<Waypoint>();
                    
        bool pathFinished = false;
        int counter = 0;
        
        Walkable last = destination;
        
        queuedWaypoints.Enqueue(new Waypoint {walkable = destination, counter = 0});
        
        while (!pathFinished)
        {
            ++counter;
            foreach (var neighbor in queuedWaypoints.Dequeue().walkable.Neighbors)
            {
                if (neighbor.Equals(_currentPosition))
                {
                    queuedWaypoints.Enqueue(new Waypoint {walkable = neighbor, counter = counter});
                    path.Enqueue(neighbor);
                    last = neighbor;
                    pathFinished = true;
                    break;
                }
        
                if (ReferenceEquals(queuedWaypoints.FirstOrDefault(x => x.walkable == neighbor), null))
                    tempQueue.Enqueue(new Waypoint {walkable = neighbor, counter = counter});
            }

            int size = tempQueue.Count;
            for (int i = 0; i < size; i++)
            {
                queuedWaypoints.Enqueue(tempQueue.Dequeue());
            }
        }

        for (int i = counter; i == 0; i--)
        {
            foreach (var neighbor in last.Neighbors)
            {
                if (queuedWaypoints.FirstOrDefault(x => x.walkable == neighbor).counter < counter)
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
        if (!ReferenceEquals(_currentDesition, SetDestination))
        {
            _currentDesition = SetDestination;
            _currentPath = GeneratePath(SetDestination);
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
