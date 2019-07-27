using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using Level;
using Level.Objects;
using Priority_Queue;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace Player {
    public class Pathfinder : MonoBehaviour
    {
        public Walkable _currentEnd;
        private Queue<Walkable> _currentPath;
        private Walkable _currentStart;
        [ShowInInspector] public static bool Navigating;
        [SerializeField] public float WalkSpeed = 5.0f;

        private void Awake()
        {
            Indicator.OnWalkableClicked += NavigateTo;
        }

        private void OnDestroy()
        {
            Indicator.OnWalkableClicked -= NavigateTo;
        }

        private void NavigateTo(Walkable destination)
        {
            var path = GeneratePath(GetCurrentWalkable(out var walk), destination);
            Debug.Log(walk.collider.name);
            if (path == null)
            {
                Debug.Log("Gaming");
                return;
            }

            _currentPath = new Queue<Walkable>(path);

            _currentEnd = _currentPath.Dequeue();
            Navigating = true;
        }

        private IEnumerable<Walkable> GeneratePath(Walkable start, Walkable destination)
        {
            if (start == null) return null;
            if (start == destination) return null;
            
            float Heuristic(Vector3 a, Vector3 b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);

            float MovementCost(Node a, Node b) => 1;

            var goalNode = destination.Node;
            var path = new Queue<Walkable>();
            var frontier = new FastPriorityQueue<Node>(MapController.NUM_WALKABLES);
            var cameFrom = new Dictionary<int, Node>();
            var costSoFar = new Dictionary<int, float> {{start.UniqueId, 0}};
            frontier.Enqueue(start.Node, 0);

            Node current;
            
            do
            {
                current = frontier.Dequeue();

                if (current == goalNode) break;

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
            } while (frontier.Count > 0);

            // retrace path
            current = goalNode;
            while (current.Walkable.UniqueId != start.UniqueId)
            {
                path.Enqueue(current.Walkable);
                try
                {
                    current = cameFrom[current.Walkable.UniqueId];
                }
                catch (KeyNotFoundException e)
                {
                    return null;
                }
            }
        
            return path.Reverse();
        }

        public Walkable GetCurrentWalkable(out RaycastHit hit) =>
            Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out hit, 2, LayerMask.GetMask("Model"))
                ? hit.transform.parent.GetComponent<Walkable>()
                : null;

        public Action OnMove;
        
        private void Update()
        {
            if (Navigating)
                if (Vector3.Distance(transform.position, _currentEnd.transform.position) > Vector3.kEpsilon)
                {                   
                    var position = _currentEnd.transform.position;

                    var vec = new Vector3(position.x,
                        position.y + .5f + transform.GetComponent<CapsuleCollider>().height / 2,
                        position.z);
                    transform.LookAt(vec, Vector3.up);
                    transform.position = Vector3.MoveTowards(transform.position, vec, WalkSpeed * .1f);
                    if (Vector3.Distance(transform.position, vec) < Vector3.kEpsilon)
                    {
                        transform.position = vec;
                        if (_currentPath.Count > 0)
                        {
                            _currentEnd = _currentPath.Dequeue();
                            OnMove?.Invoke();
                        }
                        else
                            Navigating = false;
                    }
                }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {                      
            Gizmos.color = Color.cyan;
            if (_currentPath == null) return;
            
            foreach (var walkable in _currentPath)
                Gizmos.DrawSphere(walkable.transform.position + Vector3.up / 2, .2f);
        }
#endif
    }
}