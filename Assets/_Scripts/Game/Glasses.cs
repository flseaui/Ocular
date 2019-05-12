using System;
using UnityEngine;

namespace Game {
    public enum GlassesType
    {
        One,
        Two,
        Three
    }
    
    
    /// <summary>
    /// A pair of colored glasses and their info.
    /// </summary>
    [Serializable]
    public class Glasses : IEquatable<Glasses>
    {
        /// <summary>
        /// The color of the glasses.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The type of glasses.
        /// </summary>
        public GlassesType GlassesType;

        /// <summary>
        /// Whether or not the glasses are worn.
        /// </summary>
        [NonSerialized] public bool Enabled;

        /// <summary>
        /// The key that toggles the glasses state.
        /// </summary>
        [SerializeField] public KeyCode Keybind;

        public Glasses(Color color, GlassesType type, KeyCode keybind)
        {
            Color = color;
            GlassesType = type;
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