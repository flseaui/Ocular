using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using Level.Objects;
using Player;
using Priority_Queue;
using UI;
using UnityEngine;

public class ClonePathfinder : MonoBehaviour
{
    private enum Cardinal
    {
        North,
        East,
        South,
        West,
        None
    }

    private Walkable _currentEnd;
    public float WalkSpeed;
    public bool Navigating;
    public bool AtGoal;
    
    private void Update()
    {
        if (Navigating)
        {
            if (Vector3.Distance(transform.position, _currentEnd.transform.position) > Vector3.kEpsilon)
            {
                if (AtGoal) return;
                
                var position = _currentEnd.transform.position;

                var vec = new Vector3(position.x,
                    position.y + .5f + transform.GetComponent<CapsuleCollider>().height / 2,
                    position.z);
                transform.LookAt(vec, Vector3.up);
                transform.position = Vector3.MoveTowards(transform.position, vec, WalkSpeed * .1f);
                if (Vector3.Distance(transform.position, vec) < Vector3.kEpsilon)
                {
                    transform.position = vec;
                    Navigating = false;
                }
            }
        }
    }

    public void MirrorClone(Walkable playerStart, Walkable playerEnd)
    {
        if (AtGoal) return;
        
        var playerCardinal = GetCardinal(playerStart, playerEnd);
        Cardinal targetCardinal;
        var currentWalkable = GetCurrentWalkable(out _);

        /*
        if (playerCardinal == Cardinal.South)
            targetCardinal = Cardinal.North;
        else if (playerCardinal == Cardinal.North)
            targetCardinal = Cardinal.South;
        else if (playerCardinal == Cardinal.East)
            targetCardinal = Cardinal.West;
        else if (playerCardinal == Cardinal.West)
            targetCardinal = Cardinal.East;
        else
            targetCardinal = Cardinal.None;
            */

        targetCardinal = playerCardinal;

        foreach (var neighbor in currentWalkable.Node.Neighbors)
        {
            if (GetCardinal(currentWalkable, neighbor.Walkable) == targetCardinal)
            {
                _currentEnd = neighbor.Walkable;
                Navigating = true;
                return;
            }
        }
    }

    private Cardinal GetCardinal(Walkable start, Walkable end)
    {
        if (start is null || end is null)
            return Cardinal.None;
        
        if (start.transform.position.x > end.transform.position.x)
            return Cardinal.West;
        if (start.transform.position.x < end.transform.position.x)
            return Cardinal.East;
        if (start.transform.position.z > end.transform.position.z)
            return Cardinal.South;
        if (start.transform.position.z < end.transform.position.z)
            return Cardinal.North;
        
        Debug.Log("Same as start");
        return Cardinal.None;
    }
    
    
    private Walkable GetCurrentWalkable(out RaycastHit hit) =>
        Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out hit, 2)
            ? hit.transform.parent.GetComponent<Walkable>()
            : null;
}
