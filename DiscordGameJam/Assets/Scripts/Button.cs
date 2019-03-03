using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    enum buttonType
    {
        Toggle,
        Hold
    }

    public bool state;

    private buttonType type;

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player" || col.transform.tag == "Box")
        {
            if (type == buttonType.Hold)
                state = true;
            else
                state = !state;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Player" || col.transform.tag == "Box")
        {
            if (type == buttonType.Hold)
                state = false;
        }
    }
}
