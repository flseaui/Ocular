using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public static Action<Walkable> OnWalkableClicked;
    
    private readonly int[] _slopeAngles = 
    {
        0, 270, 180, 90
    };
    
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var slope))
            {
                transform.localRotation = Quaternion.Euler(45, _slopeAngles[(int) slope.DirectionFacing], 0);
                transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y,
                    hit.transform.position.z);

                if (Input.GetMouseButtonDown(0))
                    OnWalkableClicked?.Invoke(slope);
            }
            else if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
                transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + .55f,
                    hit.transform.position.z);

                if (Input.GetMouseButtonDown(0))
                    OnWalkableClicked?.Invoke(walkable);
            }
        }
        else
        {
            transform.position = new Vector3(-100, -100, -100);
        }
    }
}
