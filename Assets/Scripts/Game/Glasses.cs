using System;
using UnityEngine;

namespace Game {
    [Serializable]
    public class Glasses : IEquatable<Glasses>
    {
        public readonly Color Color;

        [NonSerialized] public bool Enabled;

        [SerializeField] public KeyCode Keybind;

        public Glasses(Color color, KeyCode keybind)
        {
            Color = color;
            Keybind = keybind;
            Enabled = false;
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
            return obj.GetType() == GetType() && Equals((Glasses) obj);
        }

        public override int GetHashCode() => Color.GetHashCode();

        public static bool operator == (Glasses left, Glasses right) => Equals(left, right);

        public static bool operator != (Glasses left, Glasses right) => !Equals(left, right);
    }
}