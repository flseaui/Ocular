using System;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;

[Serializable]
public class Glasses : IEquatable<Glasses>
{
    public Color Color;
    [SerializeField]
    public KeyCode Keybind;
    [NonSerialized]
    public bool Enabled;

    public Glasses(Color color, KeyCode keybind)
    {
        Color = color;
        Keybind = keybind;
        Enabled = true;
    }
    
    public bool Equals(Glasses other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Color.Equals(other.Color);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Glasses) obj);
    }

    public override int GetHashCode() => Color.GetHashCode();

    public static bool operator ==(Glasses left, Glasses right) => Equals(left, right);

    public static bool operator !=(Glasses left, Glasses right) => !Equals(left, right);
}