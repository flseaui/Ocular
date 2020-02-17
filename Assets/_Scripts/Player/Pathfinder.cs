using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using Level;
using Level.Objects;
using Misc;
using Priority_Queue;
using Sirenix.OdinInspector;
using UI;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class Pathfinder : MonoBehaviour
    {
        [NonSerialized]
        public Walkable _currentEnd;
        private Queue<Walkable> _currentPath;
        private Walkable _currentStart;
        [ShowInInspector] public static bool OnStairs;
        [ShowInInspector] public static bool Navigating;
        [ShowInInspector] public static bool AtGoal;
        [SerializeField] public float WalkSpeed = 5.0f;
        private Queue<Walkable> _queuedPath = null;
        private void Awake()
        {
            Indicator.OnWalkableClicked += NavigateTo;
            LevelController.OnLevelLoaded += ClearPath;
        }

        private void OnDestroy()
        {
            Indicator.OnWalkableClicked -= NavigateTo;
        }

        private void ClearPath()
        {
            _currentPath = null;
            _currentEnd = null;
            _currentStart = null;
            Navigating = false;
        }

        private void NavigateTo(Walkable destination)
        {
            var path = GeneratePath(Navigating ? _currentEnd : GetCurrentWalkable(out _), destination);
            if (path == null)
                return;

            if (AtGoal || Player.Died) return;
            
            if (Navigating)
            {
                _queuedPath = new Queue<Walkable>(path);
            }
            else
            {
                _queuedPath = null;
                _currentPath = new Queue<Walkable>(path);

                _currentEnd = _currentPath.Dequeue();
                Navigating = true;
                
                GetComponent<Player>().ChangeFacing(GetCardinal(GetCurrentWalkable(out _), _currentEnd));
                
                Physics.Raycast( _currentEnd.transform.position, Vector3.up, out var cloneHit, 2, LayerMask.GetMask("Player"));
                if (cloneHit.collider != null)
                {
                    Navigating = false;
                    ClearPath();
                    return;
                }
            }
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
            var costSoFar = new Dictionary<int, float> { { start.UniqueId, 0 } };
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

        private void FixedUpdate()
        {
            if (Navigating)
            {
                Physics.Raycast( _currentEnd.transform.position, Vector3.up, out var cloneHit, 2, LayerMask.GetMask("Player"));
                if (cloneHit.collider != null)
                {
                    Navigating = false;
                    ClearPath();
                    return;
                }
                
                if (Vector3.Distance(transform.position, _currentEnd.transform.position) > Vector3.kEpsilon)
                {
                    var position = _currentEnd.transform.position;

                    var vec = new Vector3(position.x,
                        position.y + .5f + transform.GetComponent<CapsuleCollider>().height / 2,
                        position.z);
                    //transform.LookAt(vec, Vector3.up);
                    transform.position = Vector3.MoveTowards(transform.position, vec, WalkSpeed * Time.fixedDeltaTime);
                    if (Vector3.Distance(transform.position, vec) < Vector3.kEpsilon)
                    {
                        var curWalk = GetCurrentWalkable(out _);
                        if (_queuedPath != null && _queuedPath.Count > 0)
                        {
                            ClearPath();
                            _currentPath = _queuedPath;
                            _currentEnd = _currentPath.Dequeue();
                            GetComponent<Player>().ChangeFacing(GetCardinal(curWalk, _currentEnd));
                            Navigating = true;
                        }
                        else
                        {
                            transform.position = vec;
                            if (_currentPath.Count > 0)
                            {
                                _currentEnd = _currentPath.Dequeue();
                                
                                //TODO
                                var hit = new RaycastHit();
                                Physics.Raycast( _currentEnd.transform.position, Vector3.up, out hit, 2, LayerMask.GetMask("Player"));
                                if (_currentEnd.GetComponent<Colorable>().Outlined && _currentEnd.GetComponent<Colorable>().State == Colorable.BlockState.Invisible || hit.collider != null)
                                {
                                    Navigating = false;
                                    ClearPath();
                                }
                                else
                                {
                                    GetComponent<Player>().ChangeFacing(GetCardinal(curWalk, _currentEnd));
                                    OnMove?.Invoke();
                                }
                            }
                            else
                            {
                                Navigating = false;
                            }
                        }
                    }
                }
            }
        }

        private Player.Cardinal GetCardinal(Walkable start, Walkable end)
        {
            if (start is null || end is null)
                return Player.Cardinal.None;

            if (start.transform.position.x > end.transform.position.x)
                return Player.Cardinal.West;
            if (start.transform.position.x < end.transform.position.x)
                return Player.Cardinal.East;
            if (start.transform.position.z > end.transform.position.z)
                return Player.Cardinal.South;
            if (start.transform.position.z < end.transform.position.z)
                return Player.Cardinal.North;

            Debug.Log("Same as start");
            return Player.Cardinal.None;
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