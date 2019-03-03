using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GlassesManager.Instance.OnGlassesSwitched += UpdateBlockState;
    }

    private void UpdateBlockState()
    {
        UpdateBlockState(GlassesManager.Instance.Color);
    }
    
    private void UpdateBlockState(GlassesManager.GlassesColor glassesColor)
    {
        void DisableFloor()
        {
            if (Color == BlockColor.White) return;
            transform.Find("Waypoint").GetComponent<Waypoint>().CheckBelow(true, false);
           _floor.SetActive(false);
        }
       
        void EnableFloor()
        {
            if (Color == BlockColor.White) return;
            transform.Find("Waypoint").GetComponent<Waypoint>().CheckBelow(false, true);
            _floor.SetActive(true);
        }
        
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
    }
    
    public void SetColor(BlockColor color)
    {
        var matColor = UnityEngine.Color.black; 
        switch (color)
        {
            case BlockColor.Blue: 
                matColor = UnityEngine.Color.blue;
                break;
            case BlockColor.Red: 
                matColor = UnityEngine.Color.red;
                break;
            case BlockColor.Green: 
                matColor = UnityEngine.Color.green;
                break;
            case BlockColor.Cyan: 
                matColor = UnityEngine.Color.cyan;
                break;
            case BlockColor.Magenta: 
                matColor = UnityEngine.Color.magenta;
                break;
            case BlockColor.Yellow: 
                matColor = UnityEngine.Color.yellow;
                break;
            case BlockColor.White: 
                matColor = UnityEngine.Color.white;
                break;
        }
        
        Color = color;
        transform.GetChild(1).GetComponent<Renderer>().material.color = matColor;
        UpdateBlockState();
    }
}
