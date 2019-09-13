using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using Random = System.Random;

namespace Level.Objects
{
    public class BlockRandomizer : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> _textures;
    
        public void OnEnable()
        {
            GetComponent<Renderer>().material.mainTexture = _textures.RandomElement();
        }
    }
}