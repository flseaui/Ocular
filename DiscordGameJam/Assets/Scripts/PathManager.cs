using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public float WalkSpeed = 5.0f;

    private Stack<Vector3> _currentPath;
    private Vector3 _currentWaypointPosition;
    private float _moveTimeTotal;
    private float _moveTimeCurrent;

    public void NavigateTo(Vector3 destination)
    {
        _currentPath = new Stack<Vector3>();
        var currentNode = FindClosestWaypoint(transform.position);
        var endNode = FindClosestWaypoint(destination);
        if (currentNode == null || endNode == null || currentNode == endNode)
            return;

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
        }
    }

    public void Stop()
    {
        
    }

    private void Update()
    {
        
    }

    private Waypoint FindClosestWaypoint(Vector3 target)
    {
        GameObject closest = null;
        var closestDist = Mathf.Infinity;
        foreach (var waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            var dist = (waypoint.transform.position - target).magnitude;
            if (dist < closestDist)
            {
                closest = waypoint;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            return closest.GetComponent<Waypoint>();
        }

        return null;
    }
}