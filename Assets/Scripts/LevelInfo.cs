using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public struct BlockColor
    {
        public Color Color;
        public string Name;
    }

    public BlockColor[] LevelColors;
}
