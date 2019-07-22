using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColorWheel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var left = Input.GetKeyDown(KeyCode.Q);
        var right = Input.GetKeyDown(KeyCode.E);

        if (!(left && right))
        {
            if (left)
            {
                transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, 60), .5f);
            }

            if (right)
            {
                transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, -60), .5f);
            }
        }
        
    }
}
