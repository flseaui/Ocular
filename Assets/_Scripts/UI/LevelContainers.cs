using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelContainers : MonoBehaviour
    {

        [SerializeField]
        private List<GameObject> _levels;

        [SerializeField]
        private Transform _top, _middle, _bottom;

        [SerializeField]
        private TextMeshProUGUI _topText, _middleText, _bottomText;

        [ShowInInspector, ReadOnly]
        private int _selectedIndex;

        private Vector3 _topPos, _middlePos, _bottomPos;

        private Tween _currentTween;

        private Quaternion _initialRot;

        private bool _firstTime;

        private void Awake()
        {
            //_levels = new List<GameObject>();
        }

        public void Start()
        {
            _firstTime = true;

            _topPos = _top.transform.position;
            _middlePos = _middle.transform.position;
            _bottomPos = _bottom.transform.position;

            _selectedIndex = 0;
            _levels[0].transform.position = _middlePos;
            _levels[1].transform.position = _bottomPos;
            for (var i = 2; i < _levels.Count; ++i)
            {
                var pos = _levels[i].transform.position;
                _levels[i].transform.position = new Vector3(pos.x, pos.y - 20 * i, pos.z);
            }

            _topText.text = "";
            _middleText.text = "";
            _bottomText.text = "";

            _currentTween = _levels[0].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        public void Update()
        {
            var mouseDelta = Input.mouseScrollDelta.y;

            //transform.position = new Vector3(transform.position.x, transform.position.y + mouseDelta, transform.position.z);
            if (mouseDelta > 0)
            {
                if (_selectedIndex + 1 == _levels.Count) return;

                if (_selectedIndex > 0)
                    _levels[_selectedIndex - 1].transform.DOLocalMoveY(20, .3f);

                _levels[_selectedIndex].transform.DOMove(_topPos, .3f);

                if (_levels.Count > _selectedIndex + 1)
                    _levels[_selectedIndex + 1].transform.DOMove(_middlePos, .3f);

                if (_levels.Count > _selectedIndex + 2)
                    _levels[_selectedIndex + 2].transform.DOMove(_bottomPos, .3f);

                _levels[_selectedIndex].transform.rotation = _initialRot;
                _selectedIndex++;
            }
            else if (mouseDelta < 0)
            {
                if (_selectedIndex == 0) return;

                if (_selectedIndex > 0)
                    _levels[_selectedIndex - 1].transform.DOMove(_middlePos, .3f);

                if (_selectedIndex - 1 > 0)
                    _levels[_selectedIndex - 2].transform.DOMove(_topPos, .3f);

                _levels[_selectedIndex].transform.DOMove(_bottomPos, .3f);

                if (_levels.Count > _selectedIndex + 1)
                    _levels[_selectedIndex + 1].transform.DOLocalMoveY(-60, .3f);


                _levels[_selectedIndex].transform.rotation = _initialRot;
                _selectedIndex--;

            }

            if (mouseDelta != 0 || _firstTime)
            {
                _firstTime = false;

                if (_selectedIndex > 0)
                    _topText.text = _levels[_selectedIndex - 1].name;
                else
                    _topText.text = "";

                _middleText.text = _levels[_selectedIndex].name;

                if (_levels.Count > _selectedIndex + 1)
                    _bottomText.text = _levels[_selectedIndex + 1].name;
                else
                    _bottomText.text = "";


                _currentTween.Kill();
                _initialRot = _levels[_selectedIndex].transform.rotation;
                _currentTween = _levels[_selectedIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            }
        }
    }
}
