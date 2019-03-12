using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private readonly int[] _slopeAngles = 
    {
        0, 270, 180, 90
    };
    
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.ParentHasComponent<SlopeWalkable>(out var walkable))
            {
                transform.localRotation = Quaternion.Euler(45, _slopeAngles[(int) walkable.DirectionFacing], 0);
                transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y,
                    hit.transform.position.z);
            }
            else if (hit.transform.ParentHasComponent<Walkable>())
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
                transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + .55f,
                    hit.transform.position.z);
                
                if (Input.GetMouseButtonDown(0))
                {

                }
            }
        }
        else
        {
            transform.position = new Vector3(-100, -100, -100);
        }
    }
}
