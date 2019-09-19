using System.Collections;
using ES3Types;
using Game;
using Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animation
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animation _walk;
        [SerializeField] private Animation _idle;
        [SerializeField] private Animation _teleport;
        [SerializeField] private Animation _death;
        
        private Frame _currentFrame;
        [ShowInInspector] [ReadOnly] private Animation _currentAnimation;
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
            if (Pathfinder.AtGoal && _currentAnimation != _teleport)
            {
                DetermineAnimation();
            }
            
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
            if (_currentAnimation == _teleport && _currentFrame == null)
            {
                Pathfinder.AtGoal = false;
                Destroy(gameObject);
                GameManager.OnLevelLoad?.Invoke();
                return _teleport;
            }

            if (_currentAnimation == _death && _currentFrame == null)
            {
                GetComponent<Player.Player>().ActuallyDie();
                Player.Player.Died = false;
                return _death;
            }
            
            if (Player.Player.Died)
            {
                _loopAnimation = false;
                if(_currentAnimation != _death) _currentAnimation.Stop();
                return _death;
            }
            if (Pathfinder.AtGoal)
            {
                _loopAnimation = false;
                if(_currentAnimation != _teleport) _currentAnimation.Stop();
                return _teleport;
            }
            
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
            if (ReferenceEquals(_currentFrame, null)) return;
            GetComponentInChildren<MeshFilter>().mesh = _currentFrame._mesh;
            GetComponentInChildren<MeshRenderer>().sharedMaterial = _currentFrame._material;
            _timeRemaining = anim.CurrentFrameLength() * .00001f;
            _resolvingFrame = true;
        }
    }
}