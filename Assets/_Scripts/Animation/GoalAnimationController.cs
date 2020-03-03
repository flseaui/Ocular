using System.Collections;
using Level.Objects;
using UnityEngine;

namespace OcularAnimation
{
    public class GoalAnimationController : MonoBehaviour
    {
        [SerializeField] private NewVoxelAnimation _brokenGoal;
        [SerializeField] private NewVoxelAnimation _goalAnim;
        [SerializeField] private NewVoxelAnimation _reforming;

        private NewVoxelAnimation _currentAnimation;
        
        private MeshFilter[] _meshes;

        private Goal _goal;
        
        private void Awake()
        {
            _meshes = transform.GetComponentsInChildren<MeshFilter>();
            
            _brokenGoal.Init();
            _goalAnim.Init();
            //_reforming.Init();
            
            _currentAnimation = _goalAnim;
            
            _goal = transform.GetChild(0).GetComponent<Goal>();
            
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
            if (_goal.Satisfied && _currentAnimation != _goalAnim)
            {
                return _goalAnim;
            }

            if (!_goal.Satisfied && _currentAnimation != _brokenGoal)
            {
                return _brokenGoal;
            }

            return null;
        }
    }
}