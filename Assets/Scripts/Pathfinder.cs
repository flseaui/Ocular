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
    private Walkable _queuedDestination;
    private Walkable _currentPosition;
    private Walkable _currentDesition;
    private Queue<Walkable> _currentPath;
    private Vector3 _waypointFrom;
    private Vector3 _waypointTo;
    private float _moveTimeCurrent;
    private float _moveTimeTotal;
    public float WalkSpeed = 5.0f;
    private bool _currentlyMoving;

    private void Awake()
    {
        Indicator.OnWalkableClicked += NavigateTo;
    }

    private void NavigateTo(Walkable destination)
    {
        _queuedDestination = destination;
        _waypointTo = destination.transform.position;
        _waypointFrom = GetCurrentWalkable().transform.position;
        _moveTimeTotal = (_waypointFrom - _waypointTo).magnitude / WalkSpeed;
        _moveTimeCurrent = 0;
        _currentlyMoving = true;
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
        if (_currentDesition.transform.localPosition != _queuedDestination.transform.localPosition)
        {
            _currentDesition = _queuedDestination;
            _currentPath = GeneratePath(_currentDesition);

            foreach (var item in _currentPath)
            {
                Debug.Log(item.transform.position);
            }
        }
    }
    
   
    void Update()
    {
        _currentPosition = GetCurrentWalkable();

        if (_currentlyMoving)
        {
            //Lerp player, and if at waypoint check for new desitination
            if (_moveTimeCurrent < _moveTimeTotal)
            {
                _moveTimeCurrent += Time.deltaTime;
                if (_moveTimeCurrent > _moveTimeTotal)
                    _moveTimeCurrent = _moveTimeTotal;
                transform.localPosition = Vector3.Lerp(_waypointFrom, _waypointTo, _moveTimeCurrent / _moveTimeTotal);
            }
            else
            {
                _waypointFrom = _waypointTo;
                if (_currentPath.Count > 0)
                {
                    _waypointTo = _currentPath.Dequeue().transform.position;
                    _moveTimeCurrent = 0;
                    _moveTimeTotal = (_waypointFrom - _waypointTo).magnitude / WalkSpeed;
                }
                else
                {
                    _currentlyMoving = false;
                    _currentPath.Clear();
                    _moveTimeTotal = 0;
                    _moveTimeCurrent = 0;
                }
            }
        }
        else      
            CheckForNewDestination();
    }
}
