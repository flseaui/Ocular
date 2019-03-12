using System;
using UnityEngine;

public class SlopeWalkable : Walkable
{
    public enum Direction
    {
        Forward, Left, Back, Right
    }

    private enum Orientation
    {
        Up, Down
    }

    private readonly Direction[] _directionsClockwise =
    {
        Direction.Forward,
        Direction.Left,
        Direction.Back,
        Direction.Right
    };
    
    private readonly Vector3[] _directionsVector =
    {
        Vector3.forward,
        Vector3.left,
        Vector3.back, 
        Vector3.right
    };
    
    [SerializeField] public Direction DirectionFacing;
    [SerializeField] private Orientation _orientation;

    private Direction OppositeDirection => _directionsClockwise[((int) DirectionFacing + 2) % 4];

    private Vector3 RelativeForward => _directionsVector[(int) DirectionFacing];
    private Vector3 RelativeBack => RelativeForward * -1;
    private Vector3 RelativeLeft => _directionsVector[((int) DirectionFacing + 1) % 4];
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

        // Back & Down
        if (Physics.Raycast(transform.localPosition + RelativeBack, new Vector3(0, -1, 0), out hit, 1f))
        {
            if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                AddNeighbor(walkable);
                walkable.AddNeighbor(this);
            }
        }
        
        // Forward
        if (Physics.Raycast(transform.localPosition, RelativeForward, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var slope))
            {
                if (slope.DirectionFacing == OppositeDirection)
                {
                    AddNeighbor(slope);
                }
            }
            else if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                if (hit.transform.parent.CompareTag("Walkable"))
                    AddNeighbor(walkable);
            }
        }
        
        // Back
        if (Physics.Raycast(transform.localPosition, RelativeBack, out hit, 1))
        {
            // TODO Stairs facing eachother
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable.DirectionFacing == OppositeDirection)
                AddNeighbor(walkable);
            }
        }
        
        // Left
        if (Physics.Raycast(transform.localPosition, RelativeLeft, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable.DirectionFacing == DirectionFacing)
                    if (walkable._orientation == _orientation)
                        AddNeighbor(walkable);
            }
        }
        
        // Right
        if (Physics.Raycast(transform.localPosition, RelativeRight, out hit, 1))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                if (walkable.DirectionFacing == DirectionFacing)
                    if (walkable._orientation == _orientation)
                        AddNeighbor(walkable);
            }
        }
    }
    
#if  UNITY_EDITOR
    private void OnValidate()
    {
        if (DirectionFacing == Direction.Right) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 0, 0);
        else if (DirectionFacing == Direction.Left) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
        else if (DirectionFacing == Direction.Forward) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
        else if (DirectionFacing == Direction.Back) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 90, 0);
        
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
        Gizmos.DrawRay(transform.localPosition + RelativeBack, new Vector3(0, -1, 0));
        Gizmos.DrawRay(transform.localPosition, new Vector3(0, 1, 0));*/
    }
#endif
}
