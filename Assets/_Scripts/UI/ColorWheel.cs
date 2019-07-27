using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColorWheel : MonoBehaviour
{
    public static bool Turning;

    // Update is called once per frame
    void Update()
    {
        var left = Input.GetKeyDown(KeyCode.Q);
        var right = Input.GetKeyDown(KeyCode.E);

        if (!(left && right) && !Turning)
        {
            if (left)
            {
                Turning = true;
                var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, 60), .3f);
                rotate.onComplete += () => Turning = false;
            }

            if (right)
            {
                Turning = true;
                var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, -60), .3f);
                rotate.onComplete += () => Turning = false;
            }
        }   
    }
}
