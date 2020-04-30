using Misc;
using Unity.Mathematics;
using UnityEngine;

namespace Level.Objects {
    public class SlopeWalkable : Walkable
    {
        private readonly Direction[] _directionsClockwise =
        {
            Direction.Forward,
            Direction.Left,
            Direction.Back,
            Direction.Right
        };

        private readonly float3[] _directionsVector =
        {
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right
        };

        [SerializeField] public Orientation Orientation;

        [SerializeField] public Direction DirectionFacing;

        private Direction OppositeDirection => _directionsClockwise[((int) DirectionFacing + 2) % 4];

        private float3 RelativeForward => _directionsVector[(int) DirectionFacing];
        private float3 RelativeBack => RelativeForward * -1;
        private float3 RelativeLeft => _directionsVector[((int) DirectionFacing + 1) % 4];
        private float3 RelativeRight => RelativeLeft * -1;

        public void MatchRotation(Orientation orientation, Direction direction)
        {
            Orientation = orientation;
            DirectionFacing = direction;
            UpdateRotation();
        }
        
        public override void CheckForNeighbors()
        {
            // Up
            if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out var hit, 1))
                if (hit.transform.ParentHasComponent<Walkable>())
                    Enabled = false;

            // Back & Down
            if (Physics.Raycast(new float3(transform.position) + RelativeBack, new Vector3(0, -1, 0), out hit, 1f))
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {
                    AddNeighbor(walkable);
                    walkable.AddNeighbor(this);
                }

            // Forward
            if (Physics.Raycast(new float3(transform.position) + RelativeForward / 4, RelativeForward, out hit, 1))
            {
                if (hit.transform.ParentHasComponent<SlopeWalkable>(out var slope))
                {
                    if (slope.DirectionFacing == OppositeDirection) AddNeighbor(slope);
                }
                else if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {
                    AddNeighbor(walkable);
                }
            }

            // Back
            if (Physics.Raycast(transform.position, RelativeBack, out hit, 1))
                if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
                    if (walkable.DirectionFacing == OppositeDirection)
                        AddNeighbor(walkable);

            // Left
            if (Physics.Raycast(transform.position, RelativeLeft, out hit, 1))
                if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
                    if (walkable.DirectionFacing == DirectionFacing)
                        if (walkable.Orientation == Orientation)
                            AddNeighbor(walkable);

            // Right
            if (Physics.Raycast(transform.position, RelativeRight, out hit, 1))
                if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
                    if (walkable.DirectionFacing == DirectionFacing)
                        if (walkable.Orientation == Orientation)
                            AddNeighbor(walkable);
        }

        private void UpdateRotation()
        {
            if (DirectionFacing == Direction.Right)
                transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 0, 0);
            else if (DirectionFacing == Direction.Left)
                transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
            else if (DirectionFacing == Direction.Forward)
                transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
            else if (DirectionFacing == Direction.Back)
                transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 90, 0);

            if (Orientation == Orientation.Up)
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
            if (Orientation == Orientation.Down)
                transform.localRotation = Quaternion.Euler(180, transform.localEulerAngles.y, 0);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateRotation();
        }

        /*private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.localPosition + RelativeForward / 4, RelativeForward);
        Gizmos.DrawRay(transform.localPosition, RelativeBack);
        Gizmos.DrawRay(transform.localPosition, RelativeLeft);
        Gizmos.DrawRay(transform.localPosition, RelativeRight);
        Gizmos.DrawRay(transform.localPosition + RelativeBack, new Vector3(0, -1, 0));
        Gizmos.DrawRay(transform.localPosition, new Vector3(0, 1, 0));
    }*/
#endif
        
    }
}