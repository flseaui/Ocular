using System;
using System.Collections.Generic;
using UnityEngine;

namespace OcularAnimation
{
    [CreateAssetMenu(menuName = "Voxel Animation")]
    public class VoxelAnimation : ScriptableObject
    {
        public List<AnimFrame> Frames;
        [SerializeField]
        private bool _looping;
        private int _currentFrame;

        VoxelAnimation(List<AnimFrame> frameList)
        {
            Frames = frameList;
            _currentFrame = 0;
            _looping = false;
        }

        public AnimFrame Play()
        {
            if (_currentFrame >= Frames.Count)
            {
                if (!_looping)
                {
                    Stop();
                    return null;
                }
                _currentFrame = 0;
            }

            AnimFrame nextFrame = Frames[_currentFrame];
            _currentFrame++;
            return nextFrame;
        }

        public AnimFrame Loop()
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
            return Frames[_currentFrame - 1]._timing;
        }
    }

   [Serializable]
    public class AnimFrame
    {
        [SerializeField]
        public Mesh _mesh;
        [SerializeField]
        public Material _material;

        [SerializeField] public int _timing;

        public AnimFrame(Mesh mesh, Material mat, int timing) {
            _mesh = mesh;
            _material = mat;
            _timing = timing;
        }
    }
    

}