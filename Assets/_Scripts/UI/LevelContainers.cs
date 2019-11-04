using DG.Tweening;
using Level;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelContainers : SerializedMonoBehaviour
    {
        [SerializeField]
        private List<List<GameObject>> _levels;

        [SerializeField]
        private List<GameObject> _worldContainers;

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

        private int _prevWorldIndex;

        private int _worldIndex = 0;

        private bool _active;

        public int WorldIndex
        {
            get => _worldIndex;
            set
            {
                _prevWorldIndex = _worldIndex;
                _worldIndex = value;
            }
        }

        public void Start()
        {
            var worldLevels = _levels[WorldIndex];

            _firstTime = true;

            _topPos = _top.transform.position;
            _middlePos = _middle.transform.position;
            _bottomPos = _bottom.transform.position;

            _selectedIndex = 0;
            _levels[WorldIndex][0].transform.position = _middlePos;
            if (_levels[WorldIndex].Count > 1)
            {
                _levels[WorldIndex][1].transform.position = _bottomPos;
                for (var i = 2; i < worldLevels.Count; ++i)
                {
                    var pos = worldLevels[i].transform.position;
                    worldLevels[i].transform.position = new Vector3(pos.x, pos.y - 20 * i, pos.z);
                }
            }

            _topText.text = "";
            _middleText.text = "";
            _bottomText.text = "";

            _currentTween = worldLevels[0].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        public void Update()
        {
            if (!_active) return;

            var worldLevels = _levels[WorldIndex];

            if (Input.GetMouseButtonDown(0))
            {
                LevelController.StartingLevelIndex = (WorldIndex, _selectedIndex);
                LoadingScreen.SceneToLoad = "Game";
                LoadingScreen.Instance.Show();
            }

            var up = Input.GetKeyDown(KeyCode.UpArrow);
            var down = Input.GetKeyDown(KeyCode.DownArrow);

            var mouseDelta = Input.mouseScrollDelta.y;

            //transform.position = new Vector3(transform.position.x, transform.position.y + mouseDelta, transform.position.z);
            if (mouseDelta > 0 || up)
            {
                if (_selectedIndex + 1 == worldLevels.Count) return;

                if (_selectedIndex > 0)
                    worldLevels[_selectedIndex - 1].transform.DOLocalMoveY(20, .3f);

                worldLevels[_selectedIndex].transform.DOMove(_topPos, .3f);

                if (worldLevels.Count > _selectedIndex + 1)
                    worldLevels[_selectedIndex + 1].transform.DOMove(_middlePos, .3f);

                if (worldLevels.Count > _selectedIndex + 2)
                    worldLevels[_selectedIndex + 2].transform.DOMove(_bottomPos, .3f);

                worldLevels[_selectedIndex].transform.rotation = _initialRot;
                _selectedIndex++;
            }
            else if (mouseDelta < 0 || down)
            {
                if (_selectedIndex == 0) return;

                if (_selectedIndex > 0)
                    worldLevels[_selectedIndex - 1].transform.DOMove(_middlePos, .3f);

                if (_selectedIndex - 1 > 0)
                    worldLevels[_selectedIndex - 2].transform.DOMove(_topPos, .3f);

                worldLevels[_selectedIndex].transform.DOMove(_bottomPos, .3f);

                if (worldLevels.Count > _selectedIndex + 1)
                    worldLevels[_selectedIndex + 1].transform.DOLocalMoveY(-60, .3f);


                worldLevels[_selectedIndex].transform.rotation = _initialRot;
                _selectedIndex--;

            }

            if (mouseDelta != 0 || up || down || _firstTime)
            {
                _firstTime = false;

                if (_selectedIndex > 0)
                    _topText.text = worldLevels[_selectedIndex - 1].name;
                else
                    _topText.text = "";

                _middleText.text = worldLevels[_selectedIndex].name;

                if (worldLevels.Count > _selectedIndex + 1)
                    _bottomText.text = worldLevels[_selectedIndex + 1].name;
                else
                    _bottomText.text = "";


                _currentTween.Kill();
                _initialRot = worldLevels[_selectedIndex].transform.rotation;
                _currentTween = worldLevels[_selectedIndex].transform.DOLocalRotate(new Vector3(0, 360, 0), 10, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            }
        }

        public void Disable()
        {
            _worldContainers[_worldIndex].gameObject.SetActive(false);
            _active = false;
        }

        public void SwitchWorlds(int worldIndex)
        {
            Debug.Log(worldIndex);
            WorldIndex = worldIndex;

            if (_prevWorldIndex != -1)
                _worldContainers[_prevWorldIndex].gameObject.SetActive(false);
            _worldContainers[WorldIndex].gameObject.SetActive(true);

            _active = true;
        }
    }
}
