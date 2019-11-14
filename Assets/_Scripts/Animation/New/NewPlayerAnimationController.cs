using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace OcularAnimation.New
{
    public class NewPlayerAnimationController : SerializedMonoBehaviour
    {
        [SerializeField] private List<WeightedAnimation> _idleAnims;

        private MeshFilter[] _meshes;

        private int _idleIndex;

        private NewVoxelAnimation CurrentIdle => _idleAnims[_idleIndex].Animation;

        private NewVoxelAnimation _currentAnimation;
        
        private void Awake()
        {
            _meshes = transform.GetComponentsInChildren<MeshFilter>();

            foreach (var anim in _idleAnims)
            {
                anim.Animation.Init();
            }

            _currentAnimation = _idleAnims[0].Animation;
            StartAnim();
        }

        private void StartAnim()
        {
            StartCoroutine(NextFrame());
        }

        private void StopAnim()
        {
            StopCoroutine(NextFrame());
        }

        private IEnumerator NextFrame()
        {
            yield return new WaitForSeconds(_currentAnimation.CurrentFrame.TimingMS / 1000f);
            for (var i = 0; i < _meshes.Length; ++i)
            {
                _meshes[i].sharedMesh = _currentAnimation.CurrentFrame.Meshes[i].sharedMesh;
                _meshes[i].GetComponent<MeshRenderer>().sharedMaterial =
                    _currentAnimation.CurrentFrame.Meshes[i].GetComponent<MeshRenderer>().sharedMaterial;
            }

            _currentAnimation.NextFrame();
            
            if (_currentAnimation.LastFrame)
            {
                if (_currentAnimation.Looping)
                {
                    StartCoroutine(NextFrame());
                }
                else
                {
                    ChooseNewIdle();
                }
            }
            else
            {
                StartCoroutine(NextFrame());
            }
        }

        private void ChooseNewIdle()
        {
            var nextIdle = _idleAnims.PickWeighted();
            _currentAnimation = nextIdle.Animation;
        }
    }

    [Serializable]
    public abstract class Weighted
    {
        public double Weight;
    }
    
    [Serializable]
    public class WeightedAnimation : Weighted
    {
        public NewVoxelAnimation Animation;
    }

    public static class WeightedExtensions
    {
        public static TItem PickWeighted<TItem>(this IEnumerable<TItem> source) where TItem : Weighted
        {
            if (!source.Any())
                return default;

            var orderedSource= source.OrderBy(i => i.Weight);

            var dTotalWeight = orderedSource.Sum(i => i.Weight);

            var rand = new Random();

            while (true)
            {
                var dRandom = rand.NextDouble() * dTotalWeight;

                foreach (var item in orderedSource)
                {
                    if (dRandom < item.Weight)
                        return item;

                    dRandom -= item.Weight;
                }
            }
        }
    }
    
}
