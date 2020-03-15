using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Utils.ColorModels;
using Misc;
using UnityEngine;
using Random = System.Random;

namespace Level.Objects
{
    
    [ExecuteInEditMode]
    public class BlockRandomizer : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> _textures;
    
        public void OnEnable()
        {
            StartCoroutine(RandomizeMat());
        }

        private IEnumerator RandomizeMat()
        {
            yield return null;
            
            var renderer = GetComponent<Renderer>();
            var tempMat = new Material(renderer.sharedMaterial)
            {
                mainTexture = _textures.RandomElement()
            };
            renderer.sharedMaterial = tempMat;
        }
    }
}