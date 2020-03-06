using Level;
using Level.Objects;
using UnityEngine;

namespace Misc
{
    public class OutlineController : MonoBehaviour
    {
        private IController[] _controllers;

        private bool _showOutlines;
        private bool _firstTime;
        
        private void Awake()
        {
            LevelController.OnLevelLoaded += OnLevelLoad;
            _firstTime = true;
        }

        private void OnLevelLoad()
        {
            _controllers = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                .GetComponent<MapController>().Controllers;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _showOutlines = !_showOutlines;
    
                foreach (var controller in _controllers)
                {
                    var button = (ButtonWalkable) controller;
                    foreach (var target in button.TargetBlocks)
                    {
                        target.Outlined = _showOutlines;
                        target.UpdateState();
                        target.Outlined = _showOutlines;
                        if (!_showOutlines)
                        {
                            target.UpdateState();
                        }
                    }
                }
            }
        }
    }
}