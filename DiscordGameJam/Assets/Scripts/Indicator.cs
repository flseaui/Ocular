using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Indicator : MonoBehaviour
{
    public GameObject Player;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                Player.GetComponent<PathManager>().NavigateTo(hit.transform.position);
            }
        }
    }
}
