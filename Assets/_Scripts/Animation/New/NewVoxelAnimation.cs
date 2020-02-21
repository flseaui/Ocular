using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OcularAnimation.New
{
    [CreateAssetMenu(fileName = "VoxelAnimation", menuName = "Voxel Animation", order = 0)]
    public class NewVoxelAnimation : ScriptableObject
    {
        public float GlobalOffset;
        
        [SerializeField] private List<VoxelFrame> _frames;

        public bool Looping;
        public bool PlayOnWake;

        public VoxelFrame CurrentFrame => _frames[_currentFrame];

        [NonSerialized]
        public bool LastFrame;
        
        private int _currentFrame;

        public void Reset()
        {
            _currentFrame = 0;
            LastFrame = false;
        }
        
        public void Init()
        {
            _currentFrame = 0;
            
            foreach (var frame in _frames)
            {
                frame.FindMeshes();
            }
        }
        
        public void NextFrame()
        {
            LastFrame = false;
            if (_currentFrame + 1 == _frames.Count)
            {
                LastFrame = true;
                _currentFrame = 0;
            }
            else
                ++_currentFrame;
        }
        
    }

    [Serializable]
    public class VoxelFrame
    {
        public int TimingMS = 180;
        public int VoxelOffset;
        
        [SerializeField]
        private GameObject _voxelContainer;

        [HideInInspector]
        public MeshFilter[] Meshes;
        
        public void FindMeshes()
        {
            Meshes = _voxelContainer.GetComponentsInChildren<MeshFilter>();
        }
    }
}