using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldSpacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _worlds;

    [SerializeField] private float _spacing;

    public void Update()
    {
        float x = 0;
        foreach (var world in _worlds)
        {
           // world.GetComponent<Rend>()
        }
    }
}
