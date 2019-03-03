using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public enum GlassesColor
    {
        White,
        Red,
        Yellow,
        Blue,
        Cyan,
        Green,
        Magenta
    }

    public GlassesColor Color;

    [SerializeField] private GameObject _floor;

    private void Start()
    {
        void DisableFloor()
        {
            transform.Find("Waypoint").GetComponent<Waypoint>().CheckBelow();
            _floor.SetActive(false);
        }
        
        GlassesManager.Instance.OnGlassesSwitched += glassesColor =>
        {
            if (Color == GlassesColor.White) return;

            switch (glassesColor)
            {
                case GlassesManager.GlassesColor.Red:
                    if (Color == GlassesColor.Red || Color == GlassesColor.Yellow || Color == GlassesColor.Magenta)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Green:
                    if (Color == GlassesColor.Yellow || Color == GlassesColor.Green || Color == GlassesColor.Cyan)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Blue:
                    if (Color == GlassesColor.Blue || Color == GlassesColor.Magenta || Color == GlassesColor.Cyan)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Yellow:
                    if (Color == GlassesColor.Yellow)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Magenta:
                    if (Color == GlassesColor.Magenta)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Cyan:
                    if (Color == GlassesColor.Cyan)
                        _floor.SetActive(true);
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.White:
                        _floor.SetActive(true);
                    break;
                case GlassesManager.GlassesColor.Black:
                        DisableFloor();
                    break;
            }
        };
    }
}
