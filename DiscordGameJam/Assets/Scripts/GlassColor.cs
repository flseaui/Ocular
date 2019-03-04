using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassColor : MonoBehaviour
{
    private void Start()
    {
        GlassesManager.Instance.OnGlassesSwitched += ChangeGlassesColor;
    }

    private void ChangeGlassesColor(GlassesManager.GlassesColor color)
    {
        var matColor = UnityEngine.Color.black;
        switch (color)
        {
            case GlassesManager.GlassesColor.Red:
                matColor = Color.red;
                break;
            case GlassesManager.GlassesColor.Green:
                matColor = Color.green;
                break;
            case GlassesManager.GlassesColor.Blue:
                matColor = Color.blue;
                break;
            case GlassesManager.GlassesColor.Yellow:
                matColor = Color.yellow;
                break;
            case GlassesManager.GlassesColor.Magenta:
                matColor = Color.magenta;
                break;
            case GlassesManager.GlassesColor.Cyan:
                matColor = Color.cyan;
                break;
            case GlassesManager.GlassesColor.White:
                matColor = Color.white;
                break;
            case GlassesManager.GlassesColor.Black:
                matColor = Color.black;
                break;
        }

        transform.GetComponent<Renderer>().material.color = matColor;
    }
}
