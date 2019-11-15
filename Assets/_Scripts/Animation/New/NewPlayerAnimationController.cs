using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using JetBrains.Annotations;
using Player;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace OcularAnimation.New
{
    public class NewPlayerAnimationController : SerializedMonoBehaviour
    {
        [SerializeField] private List<WeightedAnimation> _idleAnims;

        [SerializeField] private NewVoxelAnimation _walk;
        [SerializeField] private NewVoxelAnimation _teleport;
        [SerializeField] private NewVoxelAnimation _death;
        
        private MeshFilter[] _meshes;

        private int _idleIndex;

        private bool _idle;
        private bool _nextFrame;
        
        private NewVoxelAnimation CurrentIdle => _idleAnims[_idleIndex].Animation;

        private NewVoxelAnimation _currentAnimation;
        
        private void Awake()
        {
            _meshes = transform.GetComponentsInChildren<MeshFilter>();

            _walk.Init();
            _teleport.Init();
            _death.Init();
            
            foreach (var anim in _idleAnims)
            {
                anim.Animation.Init();
            }

            _currentAnimation = _idleAnims[0].Animation;
            _idle = true;
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

        private void NewAnim()
        {
            StopAnim();
            StartAnim();
        }
        
        private void Update()
        {
            if (!_nextFrame) return;

            _nextFrame = false;
            
            if (_currentAnimation.LastFrame)
            {
                if (_currentAnimation == _teleport)
                {
                    Pathfinder.AtGoal = false;
                    Destroy(gameObject);
                    GameManager.OnLevelLoad?.Invoke();
                }
                else if (_currentAnimation == _death)
                {
                    GetComponent<Player.Player>().ActuallyDie();
                    Player.Player.Died = false;
                }
                if (!_currentAnimation.Looping)
                    ChooseNewIdle();
                else
                    NewAnim();
            }
            else
            {
                if (Player.Player.Died && _currentAnimation != _death)
                {
                    _currentAnimation = _death;
                    _idle = false;
                    NewAnim();
                }
                else if (Pathfinder.AtGoal && _currentAnimation != _teleport)
                {
                    _currentAnimation = _teleport;
                    _idle = false;
                    NewAnim();
                }
                else if (Pathfinder.Navigating && _currentAnimation != _walk)
                {
                    _currentAnimation = _walk;
                    _idle = false;
                    NewAnim();
                }
                else
                {
                    if (!_idle)
                    {
                        ChooseNewIdle();
                    }
                }
            }
        }

        private IEnumerator NextFrame()
        {
            yield return new WaitForSeconds(_currentAnimation.CurrentFrame.TimingMS / 1000f);
            _nextFrame = true;
            for (var i = 0; i < _meshes.Length; ++i)
            {
                _meshes[i].sharedMesh = _currentAnimation.CurrentFrame.Meshes[i].sharedMesh;
                _meshes[i].GetComponent<MeshRenderer>().sharedMaterial =
                    _currentAnimation.CurrentFrame.Meshes[i].GetComponent<MeshRenderer>().sharedMaterial;
            }

            _currentAnimation.NextFrame();

            StartCoroutine(NextFrame());
        }

        private void ChooseNewIdle()
        {
            var nextIdle = _idleAnims.PickWeighted();
            _idle = true;
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
