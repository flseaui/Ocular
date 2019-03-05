using System;
using UnityEngine;

public class SlopeWalkable : Walkable
{

    private enum Direction
    {
        Forward, Back, Left, Right
    }

    private enum Orientation
    {
        Up, Down
    }
    
    [SerializeField] private Direction _direction;
    [SerializeField] private Orientation _orientation;

    private Direction OppositeDirection
    {
        get
        {
            switch (_direction)
            {
                case Direction.Forward:
                    return Direction.Back;
                case Direction.Back:
                    return Direction.Forward;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private Vector3 RelativeForward
    {
        get
        {
            switch (_direction)
            {
                case Direction.Forward:
                    return Vector3.forward;
                case Direction.Back:
                    return Vector3.back;
                case Direction.Left:
                    return Vector3.left;
                case Direction.Right:
                    return Vector3.right;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    private Vector3 RelativeBack => RelativeForward * -1;
    private Vector3 RelativeLeft
    {
        get
        {
            switch (_direction)
            {
                case Direction.Forward:
                    return Vector3.left;
                case Direction.Back:
                    return Vector3.right;
                case Direction.Left:
                    return Vector3.back;
                case Direction.Right:
                    return Vector3.forward;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    private Vector3 RelativeRight => RelativeLeft * -1;
    
    public override void CheckForNeighbors()
    {
        // Up
        if (Physics.Raycast(transform.localPosition, new Vector3(0, 1, 0), out var hit, 1))
        {
            if (hit.transform.ParentHasComponent<Walkable>())
            {
                Enabled = false;
            }
        }

        // Down
        if (Physics.Raycast(transform.localPosition + RelativeBack + new Vector3(0, .2f, 0), new Vector3(0, -1, 0), out hit, 1.2f))
        {
            if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                Neighbors.Add(walkable);
            }
        }
        
        // Forward
        if (Physics.Raycast(transform.localPosition, RelativeForward, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var slope))
            {
                if (slope._direction == OppositeDirection)
                {
                    Neighbors.Add(slope);
                }
            }
            else if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                if (hit.transform.parent.CompareTag("Walkable"))
                    Neighbors.Add(walkable);
            }
        }
        // Back
        if (Physics.Raycast(transform.localPosition, RelativeBack, out hit, 1))
        {
            // TODO Stairs facing eachother
            /*if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable._direction == _direction)
                Neighbors.Add(walkable);
            }*/
        }
        // Left
        if (Physics.Raycast(transform.localPosition, RelativeLeft, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable._direction == _direction)
                    if (walkable._orientation == _orientation)
                        Neighbors.Add(walkable);
            }
        }
        // Right
        if (Physics.Raycast(transform.localPosition, RelativeRight, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable._direction == _direction)
                    if (walkable._orientation == _orientation)
                        Neighbors.Add(walkable);
            }
        }
    }

#if  UNITY_EDITOR
    private void OnValidate()
    {
        if (_direction == Direction.Right) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 0, 0);
        else if (_direction == Direction.Left) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
        else if (_direction == Direction.Forward) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
        else if (_direction == Direction.Back) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 90, 0);
        
        if (_orientation == Orientation.Up) transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        if (_orientation == Orientation.Down) transform.localRotation = Quaternion.Euler(180, transform.localEulerAngles.y, 0);
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        /*Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.localPosition, RelativeForward);
        Gizmos.DrawRay(transform.localPosition, RelativeBack);
        Gizmos.DrawRay(transform.localPosition, RelativeLeft);
        Gizmos.DrawRay(transform.localPosition, RelativeRight);
        Gizmos.DrawRay(transform.localPosition + RelativeBack + new Vector3(0, .2f, 0), new Vector3(0, -1, 0));
        Gizmos.DrawRay(transform.localPosition, new Vector3(0, 1, 0));*/
    }
#endif
}
