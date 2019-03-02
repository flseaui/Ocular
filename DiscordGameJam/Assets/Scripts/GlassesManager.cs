using System;
using TMPro;
using UnityEngine;

public class GlassesManager : Singleton<GlassesManager>
{
    public Action<bool> OnRedToggled;
    public Action<bool> OnBlueToggled;
    public Action<bool> OnGreenToggled;

    [SerializeField] private PostProcessing _glassesFilter;

    public bool RedGlasses;
    public bool GreenGlasses;
    public bool BlueGlasses;

    public void RedToggle(bool state)
    {
        RedGlasses = state;
        _glassesFilter.SetRedFilter(state);
        OnRedToggled?.Invoke(state);
    }

    public void GreenToggle(bool state)
    {
        GreenGlasses = state;
        _glassesFilter.SetGreenFilter(state);
        OnGreenToggled?.Invoke(state);
    }
    
    public void BlueToggle(bool state)
    {
        BlueGlasses = state;
        _glassesFilter.SetBlueFilter(state);
        OnBlueToggled?.Invoke(state);
    }
}
