using System.Collections;
using ES3Types;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animation
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animation _walk;
        [SerializeField] private Animation _idle;
        private Frame _currentFrame;
        private Animation _currentAnimation;
        private bool _loopAnimation;
        private bool _resolvingFrame;
        private float _timeRemaining;

        private void Start()
        {
            _currentAnimation = _idle;
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

        private Animation DetermineAnimation()
        {
            
            
            if (Pathfinder.Navigating)
            {
                _loopAnimation = true;
                if(_currentAnimation != _walk) _currentAnimation.Stop();
                return _walk;
            }
            
            _loopAnimation = true;
            if(_currentAnimation != _idle) _currentAnimation.Stop();
            return _idle;
        }

        private void PlayAnimation(Animation anim, bool loop)
        {
            _currentFrame = loop ? anim.Loop(): anim.Play();
            GetComponentInChildren<MeshFilter>().mesh = _currentFrame._mesh;
            GetComponentInChildren<MeshRenderer>().sharedMaterial = _currentFrame._material;
            _timeRemaining = anim.CurrentFrameLength() * .00001f;
            _resolvingFrame = true;
        }
    }
}