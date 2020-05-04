using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Level.Objects.Clones
{
    public abstract class AClonePathfinder : MonoBehaviour
    {
        public float WalkSpeed;
        public bool Navigating;
        public bool AtGoal;
        public bool OnStairs;
        
        [ShowInInspector, ReadOnly]
        public bool StopNavNextFrame;

        public virtual bool CanMove(Walkable start, Walkable end)
        {
            return false;
        }

        public virtual void Step(Walkable start, Walkable end)
        {
        }

        public Walkable GetCurrentWalkable(out RaycastHit hit) =>
            Physics.Raycast(transform.localPosition, new float3(0, -1, 0), out hit, 2, LayerMask.GetMask("Model"))
                ? hit.transform.parent.GetComponent<Walkable>()
                : null;

    }
}