using System.Collections;
using UnityEngine;

namespace OcularAnimation.New
{
    public class CloneGoalAnimationController : MonoBehaviour
    {
        [SerializeField] private NewVoxelAnimation _animation;

        private MeshFilter[] _meshes;
        
        private void Awake()
        {
            _meshes = transform.GetChild(0).GetComponents<MeshFilter>();
            
            if (_animation.PlayOnWake)
            {
                StartAnim();
            }
            
            _animation.Init();
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
            yield return new WaitForSeconds(_animation.CurrentFrame.TimingMS / 1000f);
            for (var i = 0; i < _meshes.Length; ++i)
            {
                _meshes[i].sharedMesh = _animation.CurrentFrame.Meshes[i].sharedMesh;
                _meshes[i].GetComponent<MeshRenderer>().sharedMaterial =
                    _animation.CurrentFrame.Meshes[i].GetComponent<MeshRenderer>().sharedMaterial;
            }

            _animation.NextFrame();
            
            if (_animation.LastFrame)
            {
                if (_animation.Looping)
                {
                    StartCoroutine(NextFrame());
                }
            }
            else
            {
                StartCoroutine(NextFrame());
            }
        }
    }
}