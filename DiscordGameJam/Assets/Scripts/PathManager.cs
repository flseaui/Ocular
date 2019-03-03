using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	private Queue<Vector3> _currentPath = new Queue<Vector3>();
	private Vector3 _waypointTo;
	private Vector3 _waypointFrom;
	private float _moveTimeCurrent;
	private float _moveTimeTotal;
	public float WalkSpeed = 5.0f;
	public bool Navigating;

	private Vector3 lastNodePos;

	private class WaypointNode
	{
		public Waypoint _waypoint;
		public WaypointNode _previous;
		public int _length;
		public bool _infinity;

		public WaypointNode(Waypoint waypoint, bool infinite)
		{
			_waypoint = waypoint;
			_infinity = infinite;
			_length = 0;
			_previous = null;
		}

		public void UpdatePrevious(WaypointNode from)
		{
			if (_infinity || from._length + 1 < _length)
			{
				_length = from._length + 1;
				_previous = from;
				_infinity = false;
			}
		}

		public bool CompareLength(WaypointNode node)
		{
			return (_infinity || _length > node._length);
		}

		public bool IsInfinite()
		{
			return _infinity;
		}

		public bool IsParentNode(Waypoint waypoint)
		{
			return waypoint.transform.position == _waypoint.transform.position;
		}

		public List<WaypointNode> FindAdjacent(List<WaypointNode> waypointNodes)
		{
			List<WaypointNode> returnNodes = new List<WaypointNode>();
			foreach (var neighbor in _waypoint.Neighbors)
			{
				foreach (var currentWaypointNode in waypointNodes)
				{
					if (currentWaypointNode.IsParentNode(neighbor))
					{
						returnNodes.Add(currentWaypointNode);
						break;
					}
				}
			}

			return returnNodes;
		}

		public void GetPath(Queue<Vector3> path)
		{
			if (_previous != null)
				_previous.GetPath(path);
			Debug.Log(_waypoint.transform.position);
			path.Enqueue(_waypoint.transform.position);
		}
	}

	public void NavigateToClosest(Vector3 destination)
	{
		NavigateTo(FindClosestWaypoint(destination));
	}

	public void NavigateTo(Waypoint endNode)
	{
		/*
		Debug.Log("currentNode: " + currentNode.transform.parent.name);
        Debug.Log("endNode: " + endNode.transform.parent.name);
        if (currentNode == null || endNode == null || currentNode == endNode)
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
		*/

		Waypoint currentWaypoint;
		if (Navigating)
		{
			_currentPath.Clear();
			currentWaypoint = FindClosestWaypoint(_waypointTo);
		}
		else
		{
			currentWaypoint = FindClosestWaypoint(transform.position);
		}

		WaypointNode currentNode = new WaypointNode(currentWaypoint, false);
		List<WaypointNode> waypoints = new List<WaypointNode>();
		foreach (var gameObject in GameObject.FindGameObjectsWithTag("Waypoint"))
		{
			Waypoint waypoint = gameObject.GetComponent<Waypoint>();
			if (waypoint.transform.position != currentWaypoint.transform.position)
				waypoints.Add(new WaypointNode(gameObject.GetComponent<Waypoint>(), true));
		}

		bool endFound = false;
		while (!currentNode.IsInfinite() && !endFound)
		{
			foreach (var neighbor in currentNode.FindAdjacent(waypoints))
			{
				if (neighbor._waypoint.Enabled)
				{
					if (neighbor.IsParentNode(endNode))
					{
						neighbor.UpdatePrevious(currentNode);
						neighbor.GetPath(_currentPath);
						endFound = true;
						break;
					}
					else
					{
						neighbor.UpdatePrevious(currentNode);
						waypoints.Remove(neighbor);
						int i = -1;
						while (!waypoints[++i].CompareLength(neighbor) && i < waypoints.Count) ;
						waypoints.Insert(i, neighbor);
					}
				}
			}
			currentNode = waypoints[0];
			waypoints.RemoveAt(0);
		}
		if (_currentPath.Count > 0)
		{
			if (!Navigating)
			{
				_waypointFrom = currentWaypoint.transform.position;
				_waypointTo = _currentPath.Dequeue();
				_moveTimeTotal = (_waypointFrom - _waypointTo).magnitude / WalkSpeed;
			}
			Navigating = true;
		}
	}

	public void Stop()
	{
		Navigating = false;
		_currentPath.Clear();
		_moveTimeTotal = 0;
		_moveTimeCurrent = 0;
	}

	private void Update()
	{
		if (Navigating)
		{
			if (_moveTimeCurrent < _moveTimeTotal)
			{
				_moveTimeCurrent += Time.deltaTime;
				if (_moveTimeCurrent > _moveTimeTotal)
					_moveTimeCurrent = _moveTimeTotal;
				transform.position = Vector3.Lerp(_waypointFrom, _waypointTo, _moveTimeCurrent / _moveTimeTotal);
			}
			else
			{
				_waypointFrom = _waypointTo;
				if (_currentPath.Count > 0)
				{
					_waypointTo = _currentPath.Dequeue();
					_moveTimeCurrent = 0;
					_moveTimeTotal = (_waypointFrom - _waypointTo).magnitude / WalkSpeed;
				}
				else
					Stop();
			}
		}
	}

	public Waypoint FindClosestWaypoint(Vector3 target)
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
