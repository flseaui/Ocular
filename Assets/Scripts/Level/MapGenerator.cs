using Level.Objects;
using UnityEngine;

namespace Level {
    public class MapGenerator : MonoBehaviour
    {
        public static int NUM_WALKABLES;

        public void FindNeighbors()
        {
            NUM_WALKABLES = 0;
            foreach (var walkable in transform.GetComponentsInChildren<Walkable>())
            {
                ++NUM_WALKABLES;
                walkable.CheckForNeighbors();
            }

            foreach (var colorable in transform.GetComponentsInChildren<Colorable>()) colorable.Initialize();
        }
    }
}