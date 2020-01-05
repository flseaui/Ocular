using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils.ColorModels;
using Game;
using Level.Objects;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace OcularAnimation.New
{
    public class CloneAnimationController : SerializedMonoBehaviour
    {
        [SerializeField] private List<WeightedAnimation> _idleAnims;

        [SerializeField] private NewVoxelAnimation _walk;
        [SerializeField] private NewVoxelAnimation _teleport;
        [SerializeField] private NewVoxelAnimation _death;
        [SerializeField] private NewVoxelAnimation _falling;
        
        private MeshFilter[] _meshes;

        private int _idleIndex;

        private bool _idle;
        
        private NewVoxelAnimation CurrentIdle => _idleAnims[_idleIndex].Animation;

        private NewVoxelAnimation _currentAnimation;

        private ClonePathfinder _pathfinder;
        private Clone _clone;
        
        private void Awake()
        {
            _clone = GetComponent<Clone>();
            _pathfinder = GetComponent<ClonePathfinder>();
            _meshes = transform.GetComponentsInChildren<MeshFilter>();

            _walk.Init();
            _teleport.Init();
            _death.Init();
            _falling.Init();

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

            StartCoroutine(NextFrame());
        }

        private void Update()
        {
            var newAnim = DetermineAnimation();
            if (newAnim != null)
            {
                _currentAnimation.Reset();
                _currentAnimation = newAnim;
            }

            if (_currentAnimation.LastFrame)
            {
                _idle = false;
                
                if (_currentAnimation == _teleport)
                {
                    _currentAnimation.Reset();
                    _pathfinder.AtGoal = false;
                    Destroy(gameObject);
                    //GameManager.OnLevelLoad?.Invoke();
                }
                else if (_currentAnimation == _death)
                {
                    GetComponent<Clone>().ActuallyDie();
                    _clone.Died = false;
                }
            }
        }
        
        private NewVoxelAnimation DetermineAnimation()
        {
            if (_clone.Died && _currentAnimation != _death)
            {
                _idle = false;
                return _death;
            }

            if (_pathfinder.AtGoal && _currentAnimation != _teleport)
            {
                _idle = false;
                return _teleport;
            }

            if (_clone.Falling && _currentAnimation != _falling)
            {
                //_idle = false;
                //return _falling;
            }
            
            if (_pathfinder.Navigating && _currentAnimation != _walk)
            {
                _idle = false;
                return _walk;
            }

            if (!_pathfinder.Navigating && !_pathfinder.AtGoal && !_clone.Died && !_idle)
            {
                _idle = true;
                return _idleAnims.PickWeighted().Animation;
            }

            return null;
        }
    }
}
