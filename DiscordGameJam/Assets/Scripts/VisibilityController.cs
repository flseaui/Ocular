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
    
    void Start()
    {
        GlassesManager.Instance.OnGlassesSwapped += () =>
        {
            if (Color == GlassesColor.White) return;
            switch (GlassesManager.Instance.CurrentGlasses)
            {
                case GlassesManager.Glasses.Red:
                    if (Color == GlassesColor.Red || Color == GlassesColor.Yellow || Color == GlassesColor.Magenta)
                        _floor.SetActive(true);
                    else
                        _floor.SetActive(false);
                    break;
                case GlassesManager.Glasses.Green:
                    if (Color == GlassesColor.Green || Color == GlassesColor.Yellow)
                        _floor.SetActive(true);
                    else
                        _floor.SetActive(false);
                    break;
                case GlassesManager.Glasses.Blue:
                    if (Color == GlassesColor.Blue || Color == GlassesColor.Cyan)
                        _floor.SetActive(true);
                    else
                        _floor.SetActive(false);
                    break;
            }
        };
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
