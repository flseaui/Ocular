using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OcularAnimation
{
    public class LevelAnimationController : MonoBehaviour
    {
        [SerializeField]
        private VoxelAnimation _animation;

        private AnimFrame _currentFrame;
        [ShowInInspector] [ReadOnly] private VoxelAnimation _currentAnimation;
        private bool _loopAnimation;
        private bool _resolvingFrame;
        private float _timeRemaining;

        private void Start()
        {
            _currentAnimation = _animation;
            _resolvingFrame = false;
        }

        private void Update()
        {
            if (_resolvingFrame)
            {
                _timeRemaining -= Time.deltaTime;
                if (_timeRemaining <= 0) _resolvingFrame = false;
                return;
            }
            _currentAnimation = DetermineAnimation();
            PlayAnimation(_currentAnimation, _loopAnimation);
        }

        private VoxelAnimation DetermineAnimation()
        {
            _loopAnimation = true;
            return _animation;
        }

        private void PlayAnimation(VoxelAnimation anim, bool loop)
        {
            if (!transform.GetChild(0).gameObject.activeSelf)
                return;
            _currentFrame = loop ? anim.Loop() : anim.Play();
            if (ReferenceEquals(_currentFrame, null)) return;
            GetComponentInChildren<MeshFilter>().mesh = _currentFrame._mesh;
            GetComponentInChildren<MeshRenderer>().sharedMaterial = _currentFrame._material;
            _timeRemaining = anim.CurrentFrameLength() * .00001f;
            _resolvingFrame = true;
        }

    }
}
