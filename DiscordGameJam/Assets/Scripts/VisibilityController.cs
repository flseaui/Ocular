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
        GlassesManager.Instance.OnRedToggled += state =>
        {
            if (Color == GlassesColor.White) return;
            
            if (state)
            {
                if (Color == GlassesColor.Red || Color == GlassesColor.Yellow || Color == GlassesColor.Magenta)
                    _floor.SetActive(true);
                else
                    _floor.SetActive(false);
            }
        };
        GlassesManager.Instance.OnGreenToggled += state =>
        {
            if (Color == GlassesColor.White) return;
            
            if (state)
            {
                if (Color == GlassesColor.Green || Color == GlassesColor.Yellow)
                    _floor.SetActive(true);
                else
                    _floor.SetActive(false);
            }
        };
        GlassesManager.Instance.OnBlueToggled += state =>
        {
            if (Color == GlassesColor.White) return;
            
            if (state)
            {
                if (Color == GlassesColor.Blue || Color == GlassesColor.Cyan)
                    _floor.SetActive(true);
                else
                    _floor.SetActive(false);
            }
        };
    }
}
