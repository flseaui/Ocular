using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Stack<Vector3> _currentPath;
    private Vector3 _currentWaypointPosition;
    private float _moveTimeCurrent;
    private float _moveTimeTotal;
    public float WalkSpeed = 5.0f;
    public bool Navigating;

    public void NavigateToClosest(Vector3 destination)
    {
        NavigateTo(FindClosestWaypoint(destination));
    }
    
    public void NavigateTo(Waypoint endNode)
    {
        if (Navigating) return;
        _currentPath = new Stack<Vector3>();
        var currentNode = FindClosestWaypoint(transform.position);
        Debug.Log("currentNode: " + currentNode.transform.parent.name);
        Debug.Log("endNode: " + endNode.transform.parent.name);
        if (currentNode == null || endNode == null || currentNode == endNode)
            return;
        if (endNode.Neighbors.Count == 0)
            return;
        Navigating = true;
        var openList = new SortedList<float, Waypoint>();
        var closedList = new List<Waypoint>();
        openList.Add(0, currentNode);
        currentNode.Previous = null;
        currentNode.Distance = 0f;
        while (openList.Count > 0)
        {
            currentNode = openList.Values[0];
            openList.RemoveAt(0);
            var dist = currentNode.Distance;
            closedList.Add(currentNode);
            if (currentNode == endNode) break;
            foreach (var neighbor in currentNode.Neighbors)
            {
                if (neighbor == null)
                    continue;
                if (!neighbor.Enabled)
                    continue;
                if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
                    continue;
                neighbor.Previous = currentNode;
                neighbor.Distance = dist + (neighbor.transform.position - currentNode.transform.position).magnitude;
                var distanceToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;
                if (!openList.ContainsKey(neighbor.Distance + distanceToTarget))
                    openList.Add(neighbor.Distance + distanceToTarget, neighbor);
            }
        }

        if (currentNode == endNode)
        {
            while (currentNode.Previous != null)
            {
                _currentPath.Push(currentNode.transform.position);
                currentNode = currentNode.Previous;
            }

            _currentPath.Push(transform.position);
        }
        else
        {
            Navigating = false;
        }
    }

    public void Stop()
    {
        Navigating = false;
        _currentPath = null;
        _moveTimeTotal = 0;
        _moveTimeCurrent = 0;
    }

    private void Update()
    {
        if (_currentPath != null && _currentPath.Count > 0)
        {
            if (_moveTimeCurrent < _moveTimeTotal)
            {
                _moveTimeCurrent += Time.deltaTime;
                if (_moveTimeCurrent > _moveTimeTotal)
                    _moveTimeCurrent = _moveTimeTotal;
                transform.position = Vector3.Lerp(_currentWaypointPosition, _currentPath.Peek(), _moveTimeCurrent / _moveTimeTotal);
            }
            else
            {
                _currentWaypointPosition = _currentPath.Pop();
                if (_currentPath.Count == 0)
                {
                    Stop();
                }
                else
                {
                    _moveTimeCurrent = 0;
                    _moveTimeTotal = (_currentWaypointPosition - _currentPath.Peek()).magnitude / WalkSpeed;
                }
            }
        }
    }

    private Waypoint FindClosestWaypoint(Vector3 target)
    {
        GameObject closest = null;
        var closestDist = Mathf.Infinity;
        foreach (var waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (!waypoint.GetComponent<Waypoint>().Enabled) continue;
            var dist = (waypoint.transform.position - target).magnitude;
            if (dist < closestDist)
            {
                closest = waypoint;
                closestDist = dist;
            }
        }

        if (closest != null) return closest.GetComponent<Waypoint>();
        return null;
    }
}