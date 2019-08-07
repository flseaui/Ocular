using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    [Serializable]
    public class Frame
    {
        [SerializeField]
        private Mesh _mesh;
        [SerializeField]
        private Material _material;

        [SerializeField] private int _timing;
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

        Frame Play()
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

        void Loop()
        {
            _looping = true;            
        }

        void Stop()
        {
            _currentFrame = 0;
            _looping = false;
        }
    }
}