using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class WorldContainer : MonoBehaviour
    {

        [SerializeField] private List<GameObject> _worlds;

        private int _worldIndex = 0;

        private Tween _currentTween;
        
        public void NextWorld()
        {
            _worlds[_worldIndex].transform.rotation = Quaternion.Euler(Vector3.zero);
            _currentTween.Kill();
            ++_worldIndex;
            if (_worldIndex > _worlds.Count - 1)
                _worldIndex = 0;
            var nextWorld = _worlds[_worldIndex].transform.localPosition;
            transform.DOMove(new Vector3(-nextWorld.x, transform.localPosition.y, -nextWorld.z), .5f);
            _currentTween = _worlds[_worldIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        public void PreviousWorld()
        {
            _worlds[_worldIndex].transform.rotation = Quaternion.Euler(Vector3.zero);
            _currentTween.Kill();
            --_worldIndex;
            if (_worldIndex < 0)
                _worldIndex = _worlds.Count - 1;
            var prevWorld = _worlds[_worldIndex].transform.localPosition;
            transform.DOMove(new Vector3(-prevWorld.x, transform.localPosition.y, -prevWorld.z), .5f);
            _currentTween = _worlds[_worldIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}