using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;

namespace OcularAnimation.New
{
    public class NewPlayerAnimationController : SerializedMonoBehaviour
    {
        [SerializeField] private List<WeightedAnimation> _idleAnims;

        [SerializeField] private NewVoxelAnimation _walk;
        [SerializeField] private NewVoxelAnimation _stairWalk;
        [SerializeField] private NewVoxelAnimation _teleport;
        [SerializeField] private NewVoxelAnimation _death;
        [SerializeField] private NewVoxelAnimation _falling;
        
        private MeshFilter[] _meshes;
        
        private int _idleIndex;

        private bool _idle;
        
        private NewVoxelAnimation CurrentIdle => _idleAnims[_idleIndex].Animation;

        private NewVoxelAnimation _currentAnimation;

        private void Awake()
        {
            _meshes = transform.GetComponentsInChildren<MeshFilter>();

            _walk.Init();
            _stairWalk.Init();
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

                var localPos = _meshes[i].gameObject.transform.localPosition;
                _meshes[i].gameObject.transform.localPosition = new Vector3(localPos.x, _currentAnimation.GlobalOffset + _currentAnimation.CurrentFrame.VoxelOffset * .1f, localPos.z);
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
                Debug.Log(_currentAnimation.name);
            }

            if (_currentAnimation.LastFrame)
            {
                _idle = false;
                
                if (_currentAnimation == _teleport)
                {
                    Player.Player.Teleporting = false;
                    _currentAnimation.Reset();
                    Pathfinder.AtGoal = false;
                    Destroy(gameObject);
                    GameManager.OnLevelLoad?.Invoke();
                }
                else if (_currentAnimation == _death)
                {
                    GetComponent<Player.Player>().ActuallyDie();
                    Player.Player.Died = false;
                }
            }
        }
        
        private NewVoxelAnimation DetermineAnimation()
        {
            if (Player.Player.Died && _currentAnimation != _death)
            {
                _idle = false;
                return _death;
            }

            if (Pathfinder.AtGoal && _currentAnimation != _teleport)
            {
                _idle = false;
                Player.Player.Teleporting = true;
                return _teleport;
            }

            if (Player.Player.Falling && Player.Player.FallingTimer > 1.4 && _currentAnimation != _death)
            {
                _idle = false;
                return _falling;
            }
            
            if (Pathfinder.Navigating && Pathfinder.OnStairs && _currentAnimation != _stairWalk)
            {
                Debug.Log("STAIRS");
                _idle = false;
                return _stairWalk;
            }
            
            if (Pathfinder.Navigating && _currentAnimation != _walk)
            {
                _idle = false;
                return _walk;
            }

            if (!Pathfinder.Navigating && !Pathfinder.AtGoal && !Player.Player.Died && !_idle)
            {
                _idle = true;
                return _idleAnims.PickWeighted().Animation;
            }

            return null;
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
