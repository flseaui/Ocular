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
    [SerializeField] private Walkable _currentPosition;

    private Queue<Walkable> GeneratePath(Walkable destination)
    {
        Queue<Walkable> path = new Queue<Walkable>();
        Queue<Waypoint> queuedWaypoints = new Queue<Waypoint>();
        Queue<Waypoint> tempQueue = new Queue<Waypoint>();
            
        queuedWaypoints.Enqueue(new Waypoint {walkable = destination, counter = 0});
        
        bool _pathFinished = false;

        int _counter = 0;
        
        Walkable _last = destination;
        
        //First run through and give each waypoint a rating for how many waypoints it is away from the destination
        while (!_pathFinished)
        {
            ++_counter;
            foreach (var neighbor in queuedWaypoints.Dequeue().walkable.Neighbors)
            {
                if (neighbor.Equals(_currentPosition))
                {
                    queuedWaypoints.Enqueue(new Waypoint {walkable = neighbor, counter = _counter});
                    path.Enqueue(neighbor);
                    _last = neighbor;
                    _pathFinished = true;
                    break;
                }
        
                if (queuedWaypoints.FirstOrDefault(x => x.walkable == neighbor).Equals(null))
                    tempQueue.Enqueue(new Waypoint {walkable = neighbor, counter = _counter});
            }

            int size = tempQueue.Count;
            for (int i = 0; i < size; i++)
            {
                queuedWaypoints.Enqueue(tempQueue.Dequeue());
            }
        }

        

        for (int i = _counter; i == 0; i--)
        {
            foreach (var neighbor in _last.Neighbors)
            {
                if (queuedWaypoints.FirstOrDefault(x => x.walkable == neighbor).counter < _counter)
                {
                    path.Enqueue(neighbor);
                    _last = neighbor;
                    break;
                }
            }          
        }

        return path;
    }
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
