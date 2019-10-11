using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoShadows : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().receiveShadows = false;
    }
}
