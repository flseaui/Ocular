using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    [Serializable]
    public class Frame
    {
        [SerializeField]
        public Mesh _mesh;
        [SerializeField]
        public Material _material;

        [SerializeField] public int _timing;
    }
    
    [CreateAssetMenu(menuName = "Voxel Animation")]
    public class Animation : ScriptableObject
    {
        [SerializeField]
        private List<Frame> _frames;
        [SerializeField]
        private bool _looping;
        private int _currentFrame;

        Animation(List<Frame> frameList)
        {
            _frames = frameList;
            _currentFrame = 0;
            _looping = false;
        }

        public Frame Play()
        {
            if (_currentFrame >= _frames.Count)
            {
                if (!_looping)
                {
                    Stop();
                    return null;
                }
                _currentFrame = 0;
            }

            Frame nextFrame = _frames[_currentFrame];
            _currentFrame++;
            return nextFrame;
        }

        public Frame Loop()
        {
            _looping = true;
            return Play();
        }

        public void Stop()
        {
            _currentFrame = 0;
            _looping = false;
        }

        public int CurrentFrameLength()
        {
            return _frames[_currentFrame - 1]._timing;
        }
    }
}