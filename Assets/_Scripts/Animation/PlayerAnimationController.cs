using System.Collections;
using Player;
using UnityEngine;

namespace Animation
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private bool _resolvingFrame = false;
        [SerializeField] private Animation walk;
        private Frame _currentFrame;
        
        private void Update()
        {
            if (Pathfinder.Navigating && !_resolvingFrame)
            {
                _currentFrame = walk.Loop();
                GetComponentInChildren<MeshFilter>().mesh = _currentFrame._mesh;
                GetComponentInChildren<MeshRenderer>().sharedMaterial = _currentFrame._material;
                Timer(walk.CurrentFrameLength());
            }
            else if(!Pathfinder.Navigating)
            {
                walk.Stop();
            }
        }

        IEnumerator Timer(int miliseconds)
        {
            _resolvingFrame = true;
            yield return new WaitForSeconds(miliseconds / 1000);
            _resolvingFrame = false;
        }
    }
}