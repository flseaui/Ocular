using System;
using TMPro;
using UnityEngine;

public class GlassesManager : Singleton<GlassesManager>
{
    public enum GlassesColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Magenta,
        Cyan,
        White,
        Black
    }
    
    public Action<GlassesColor> OnGlassesSwitched;

    public GlassesColor Color;
    
    [SerializeField] private PostProcessing _glassesFilter;

    [NonSerialized]
    public bool RedGlasses = true;
    [NonSerialized]
    public bool GreenGlasses = true;
    [NonSerialized]
    public bool BlueGlasses = true;

    private GlassesColor CalculateColor()
    {
        if (RedGlasses && GreenGlasses && BlueGlasses) return GlassesColor.White;
        if (RedGlasses && GreenGlasses) return GlassesColor.Yellow;
        if (RedGlasses && BlueGlasses) return GlassesColor.Magenta;
        if (GreenGlasses && BlueGlasses) return GlassesColor.Cyan;
        if (GreenGlasses) return GlassesColor.Green;
        if (BlueGlasses) return GlassesColor.Blue;
        if (RedGlasses) return GlassesColor.Red;
        return GlassesColor.Black;
    }
    
    public void RedToggle(bool state)
    {
        RedGlasses = state;
        _glassesFilter.SetRedFilter(state);
        Color = CalculateColor();
        OnGlassesSwitched?.Invoke(Color);
    }

    public void GreenToggle(bool state)
    {
        GreenGlasses = state;
        _glassesFilter.SetGreenFilter(state);
        Color = CalculateColor();
        OnGlassesSwitched?.Invoke(Color);
    }
    
    public void BlueToggle(bool state)
    {
        BlueGlasses = state;
        _glassesFilter.SetBlueFilter(state);
        Color = CalculateColor();
        OnGlassesSwitched?.Invoke(Color);
    }
}
