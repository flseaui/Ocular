using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public void FindNeighbors()
    {
        foreach (var walkable in transform.GetComponentsInChildren<Walkable>())
        {
            walkable.CheckForNeighbors();
        }
        foreach (var colorable in transform.GetComponentsInChildren<Colorable>())
        {
            colorable.Initialize();
        }
    }
}
