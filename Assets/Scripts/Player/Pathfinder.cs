using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using Priority_Queue;
using UI;
using UnityEngine;

namespace Player {
    public class Pathfinder : MonoBehaviour
    {
        private Walkable _currentEnd;
        private Queue<Walkable> _currentPath;
        private Walkable _currentStart;
        public bool Navigating;
        public float WalkSpeed = 5.0f;

        private void Awake()
        {
            Indicator.OnWalkableClicked += NavigateTo;
        }

        private void NavigateTo(Walkable destination)
        {
            var path = GeneratePath(GetCurrentWalkable(), destination);
            if (path == null)
                return;
            _currentPath = new Queue<Walkable>(path);
            _currentEnd = _currentPath.Dequeue();
            Navigating = true;
        }

        private IEnumerable<Walkable> GeneratePath(Walkable start, Walkable destination)
        {
            float Heuristic(Vector3 a, Vector3 b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);

            float MovementCost(Node a, Node b) => 1;

            if (start == destination)
                return null;

            var goalNode = destination.Node;
            var path = new Queue<Walkable>();
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

                    if (!costSoFar.ContainsKey(neighbor.Walkable.UniqueId) ||
                        newCost < costSoFar[neighbor.Walkable.UniqueId])
                    {
                        costSoFar[neighbor.Walkable.UniqueId] = newCost;
                        frontier.Enqueue(neighbor,
                            newCost + Heuristic(goalNode.Walkable.transform.localPosition,
                                neighbor.Walkable.transform.localPosition));
                        cameFrom.Add(neighbor.Walkable.UniqueId, current);
                    }
                }
            }

            // retrace path
            current = goalNode;
            while (current.Walkable.UniqueId != start.UniqueId)
            {
                path.Enqueue(current.Walkable);
                current = cameFrom[current.Walkable.UniqueId];
            }

            return path.Reverse();
        }

        private Walkable GetCurrentWalkable() =>
            Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out var hit, 2)
                ? hit.transform.parent.GetComponent<Walkable>()
                : null;

        private void Update()
        {
            if (Navigating)
                if (transform.position != _currentEnd.transform.position)
                {
                    var vec = new Vector3(_currentEnd.transform.position.x,
                        _currentEnd.transform.position.y + 0.5f + transform.GetComponent<CapsuleCollider>().height / 2,
                        _currentEnd.transform.position.z);

                    transform.position = Vector3.MoveTowards(transform.position, vec, WalkSpeed * .1f);

                    if (Vector3.Distance(transform.position, vec) < Vector3.kEpsilon)
                    {
                        transform.position = vec;
                        if (_currentPath.Count > 0)
                            _currentEnd = _currentPath.Dequeue();
                        else
                            Navigating = false;
                    }
                }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (_currentPath != null)
                foreach (var walkable in _currentPath)
                    Gizmos.DrawCube(walkable.transform.position + Vector3.up / 2, new Vector3(.5f, .5f, .5f));
        }
#endif
    }
}