using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class WorldContainer : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _worldTitle;
        
        [SerializeField] private List<GameObject> _worlds;

        private int _worldIndex;

        private Tween _currentTween;

        public void NextWorld()
        {
            //_worlds[_worldIndex].transform.rotation = Quaternion.Euler(Vector3.zero);
            _currentTween.Kill();
            ++_worldIndex;
            if (_worldIndex > _worlds.Count - 1)
                _worldIndex = 0;
            var nextWorld = _worlds[_worldIndex].transform.localPosition;
            transform.DOMove(new Vector3(-nextWorld.x, transform.localPosition.y, -nextWorld.z), .5f);
            _currentTween = _worlds[_worldIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            _worldTitle.text = _worlds[_worldIndex].name;
        }

        public void PreviousWorld()
        {
            //_worlds[_worldIndex].transform.rotation = Quaternion.Euler(Vector3.zero);
            _currentTween.Kill();
            --_worldIndex;
            if (_worldIndex < 0)
                _worldIndex = _worlds.Count - 1;
            var prevWorld = _worlds[_worldIndex].transform.localPosition;
            transform.DOMove(new Vector3(-prevWorld.x, transform.localPosition.y, -prevWorld.z), .5f);
            _currentTween = _worlds[_worldIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            _worldTitle.text = _worlds[_worldIndex].name;
        }

        public void ChooseWorld()
        {
            transform.DOScale(new Vector3(10, 10, 10), 3).onUpdate += () =>
            {
                if (transform.localScale.x >= 3 && transform.localScale.x <= 3.1)
                {
                    SceneManager.LoadSceneAsync("Game");
                }
            };
        }

        private void Start()
        {
            _currentTween = _worlds[_worldIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            _worldTitle.text = _worlds[_worldIndex].name;
        }
        
    }
}