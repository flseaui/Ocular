using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class ColorWheel : MonoBehaviour
{
    public static bool Turning;

    private bool _disabled;
    // Update is called once per frame
    void Update()
    {
        var left = Input.GetKeyDown(KeyCode.Q);
        var right = Input.GetKeyDown(KeyCode.E);

        if (Pathfinder.Navigating)
        {
            if (!_disabled)
            {
                GetComponent<Image>().DOColor(new Color(130, 130, 130, 114), .1f);
                _disabled = true;
            }
        }
        else
        {
            if (_disabled)
            {
                GetComponent<Image>().DOColor(new Color(255, 255, 255, 255), .1f);
                _disabled = false;
            }
            if (!(left && right) && !Turning)
            {
                if (left)
                {
                    Turning = true;
                    var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, 60),
                        .3f);
                    rotate.onComplete += () => Turning = false;
                }

                if (right)
                {
                    Turning = true;
                    var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, -60),
                        .3f);
                    rotate.onComplete += () => Turning = false;
                }
            }
        }
    }
}
