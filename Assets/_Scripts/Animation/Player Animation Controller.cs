using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    internal abstract class Frame
    {
        private MeshFilter _mesh;
        private Material _material;
    }

    struct Animation
    {
        private List<Frame> _frames;
        private List<int> _timing;
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

    public class PlayerAnimationController : MonoBehaviour
    {
        
        
    }
}