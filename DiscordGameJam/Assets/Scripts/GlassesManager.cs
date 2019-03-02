using System;
using TMPro;
using UnityEngine;

public class GlassesManager : Singleton<GlassesManager>
{
    public enum Glasses
    {
        Red,
        Green,
        Blue
    }

    public Action OnGlassesSwapped;

    [SerializeField]
    private TextMeshProUGUI _currentGlassesText;
    
    private Glasses _currentGlasses;
    public Glasses CurrentGlasses
    {
        get => _currentGlasses;
        set
        {
            _currentGlasses = value;
            OnGlassesSwapped?.Invoke();
        }
    }

    public void SwapGlasses()
    {
        if (CurrentGlasses == Glasses.Red)
            CurrentGlasses = Glasses.Green;
        else if (CurrentGlasses == Glasses.Green)
            CurrentGlasses = Glasses.Blue;
        else
            CurrentGlasses = Glasses.Red;

        _currentGlassesText.text = $"Current Glasses: {CurrentGlasses}";
    }
    
}
