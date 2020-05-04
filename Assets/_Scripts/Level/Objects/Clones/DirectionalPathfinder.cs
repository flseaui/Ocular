using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using static Player.Player;

namespace Level.Objects.Clones
{
    public class DirectionalPathfinder : AClonePathfinder
    {
        private Walkable _currentEnd;

        private Cardinal _targetCardinal;

        private bool _stopNavTrigger;
    
        private bool _newNav;

        private int _newNavTimer;
        private int _framesToWaitForNav = 3;
    
        private void FixedUpdate()
        {
            if (_newNavTimer > 0)
                _newNavTimer--;
        
            if (_newNavTimer <= 0 && !_newNav && StopNavNextFrame)
            {
                StopNavNextFrame = false;
                Navigating = false;
            }
        
            if (_newNav)
            {
                StopNavNextFrame = false;
                _newNav = false;
            }

            if (Navigating && !StopNavNextFrame)
            {
                if (Vector3.Distance(transform.position, _currentEnd.transform.position) > Vector3.kEpsilon)
                {
                    if (AtGoal) return;
                
                    var position = _currentEnd.transform.position;

                    var vec = new float3(position.x,
                        position.y + .5f + transform.GetComponent<CapsuleCollider>().height / 2,
                        position.z);
                    transform.position = Vector3.MoveTowards(transform.position, vec, WalkSpeed * Time.fixedDeltaTime);
                    GetComponent<Clone>().ChangeFacing(_targetCardinal);
                    if (Vector3.Distance(transform.position, vec) < Vector3.kEpsilon)
                    {
                        transform.position = vec;
                        StopNavNextFrame = true;
                        _newNavTimer = _framesToWaitForNav;
                    }
                }
            }
        }

        public override bool CanMove(Walkable playerStart, Walkable playerEnd)
        {
            return true;
        }
        
        public bool CanMove(out Walkable target)
        {
            target = null;
            
            if (AtGoal) return false;
        
            var currentWalkable = GetCurrentWalkable();

            if (_targetCardinal == Cardinal.None || currentWalkable == null)
                return false;
        
            foreach (var neighbor in currentWalkable.Node.Neighbors)
            {
                if (GetCardinal(currentWalkable, neighbor.Walkable) == _targetCardinal)
                {
                    target = neighbor.Walkable;
                    return true;
                }
            }

            return false;
        }
    
        public override void Step(Walkable start, Walkable end)
        {
            if (CanMove(out var target))
            {
                _currentEnd = target;
                Navigating = true;
                _newNav = true;
            }
            else
            {
                _targetCardinal = FlipCardinal(_targetCardinal);
                if (CanMove(out target))
                {
                    _currentEnd = target;
                    Navigating = true;
                    _newNav = true;
                }
            }
        }

        private Cardinal FlipCardinal(Cardinal cardinal)
        {
            switch (cardinal)
            {
                case Cardinal.North:
                    return Cardinal.South;
                case Cardinal.East:
                    return Cardinal.West;
                case Cardinal.South:
                    return Cardinal.North;
                case Cardinal.West:
                    return Cardinal.East;
                case Cardinal.None:
                    return Cardinal.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
            }
        }

        private Cardinal GetCardinal(Walkable start, Walkable end)
        {
            if (start is null || end is null || !end.Enabled)
                return Cardinal.None;
        
            if (start.transform.position.x > end.transform.position.x)
                return Cardinal.West;
            if (start.transform.position.x < end.transform.position.x)
                return Cardinal.East;
            if (start.transform.position.z > end.transform.position.z)
                return Cardinal.South;
            if (start.transform.position.z < end.transform.position.z)
                return Cardinal.North;
        
            return Cardinal.None;
        }


        public Walkable GetCurrentWalkable() => GetCurrentWalkable(out _);

    }
}
