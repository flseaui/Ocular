using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    private Walkable[] _walkables;

    private void Start()
    {
        _walkables = transform.GetComponentsInChildren<Walkable>();
        FindNeighbors();
    }

    private void FindNeighbors()
    {
        foreach (var walkable in _walkables)
        {
            walkable.CheckForNeighbors();
        }
    }
}
