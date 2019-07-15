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
    enum Cardinal
    {
        north,
        east,
        south,
        west,
        none
    }

    private Walkable _currentEnd;
    public float WalkSpeed;
    public bool Navigating;
    
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
                    Navigating = false;
                }
            }
    }

    public void MirrorClone(Walkable playerStart, Walkable playerEnd)
    {
        Cardinal playerCardinal = GetCardinal(playerStart, playerEnd);
        Cardinal targetCardinal;
        Walkable currentWalkable = GetCurrentWalkable(out _);

        if (playerCardinal == Cardinal.south)
            targetCardinal = Cardinal.north;
        else if (playerCardinal == Cardinal.north)
            targetCardinal = Cardinal.south;
        else if (playerCardinal == Cardinal.east)
            targetCardinal = Cardinal.west;
        else if (playerCardinal == Cardinal.west)
            targetCardinal = Cardinal.east;
        else
            targetCardinal = Cardinal.none;
        

        foreach (Node neighbor in currentWalkable.Node.Neighbors)
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
        if (start.transform.position.x > end.transform.position.x)
            return Cardinal.west;
        if (start.transform.position.x < end.transform.position.x)
            return Cardinal.east;
        if (start.transform.position.z > end.transform.position.z)
            return Cardinal.south;
        if (start.transform.position.z < end.transform.position.z)
            return Cardinal.north;
        
        Debug.Log("Same as start");
        return Cardinal.none;
    }
    
    
    private Walkable GetCurrentWalkable(out RaycastHit hit) =>
        Physics.Raycast(transform.localPosition, new Vector3(0, -1, 0), out hit, 2)
            ? hit.transform.parent.GetComponent<Walkable>()
            : null;
}
