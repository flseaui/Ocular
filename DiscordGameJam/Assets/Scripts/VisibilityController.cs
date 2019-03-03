using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public enum BlockColor
    {
        White,
        Red,
        Yellow,
        Blue,
        Cyan,
        Green,
        Magenta
    }

    public BlockColor Color;

    [SerializeField] private GameObject _floor;

    private void Start()
    {
        void DisableFloor()
        {
            if (Color == BlockColor.White) return;
            transform.Find("Waypoint").GetComponent<Waypoint>().CheckBelow(true);
            
            _floor.SetActive(false);
        }
       
        void EnableFloor()
        {
            if (Color == BlockColor.White) return;
            transform.Find("Waypoint").GetComponent<Waypoint>().CheckBelow(false);
            
            _floor.SetActive(true);
        }
        
        GlassesManager.Instance.OnGlassesSwitched += glassesColor =>
        {
            switch (glassesColor)
            {
                case GlassesManager.GlassesColor.Red:
                    if (Color == BlockColor.Red || Color == BlockColor.Yellow || Color == BlockColor.Magenta)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Green:
                    if (Color == BlockColor.Yellow || Color == BlockColor.Green || Color == BlockColor.Cyan)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Blue:
                    if (Color == BlockColor.Blue || Color == BlockColor.Magenta || Color == BlockColor.Cyan)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Yellow:
                    if (Color == BlockColor.Yellow)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Magenta:
                    if (Color == BlockColor.Magenta)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.Cyan:
                    if (Color == BlockColor.Cyan)
                        EnableFloor();
                    else
                        DisableFloor();
                    break;
                case GlassesManager.GlassesColor.White:
                    EnableFloor();
                    break;
                case GlassesManager.GlassesColor.Black:
                        DisableFloor();
                    break;
            }
        };
    }
}
