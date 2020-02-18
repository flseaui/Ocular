using System.Collections;
using UnityEngine;

namespace OcularAnimation.New
{
    public class GoalAnimationController : MonoBehaviour
    {
        [SerializeField] private NewVoxelAnimation _brokenGoal;
        [SerializeField] private NewVoxelAnimation _goal;
        [SerializeField] private NewVoxelAnimation _reforming;

        private NewVoxelAnimation _currentAnimation;
        
        private MeshFilter[] _meshes;
        
        private void Awake()
        {
            _meshes = transform.GetComponentsInChildren<MeshFilter>();
            
            _brokenGoal.Init();
            _goal.Init();
            _reforming.Init();
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
            }

            _currentAnimation.NextFrame();

            StartCoroutine(NextFrame());
        }

        private void Update()
        {
            var newAnim = DetermineAnimation();
            
            if (newAnim != null && newAnim != _currentAnimation)
            {
                _currentAnimation.Reset();
                newAnim.Reset();
                _currentAnimation = newAnim;
            }
        }
        
        private NewVoxelAnimation DetermineAnimation()
        {
            return null;
        }
    }
}