using System;
using System.Collections.Generic;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Objects 
{
    public class ButtonWalkable : Walkable, IController
    {
        [SerializeField] private Button _buttonModel;

        [SerializeField, ColorPalette("RGB")] private Color _color;

        private Vector3 _initialPosition;

        [SerializeField] private List<Colorable> _targetBlocks;
        public bool State
        {
            get => _buttonModel.State;
            set => _buttonModel.State = value;
        }

        private void Start()
        {
            if (_buttonModel == null)
                transform.Find("ButtonModel").gameObject.AddComponent<Button>();

            _initialPosition = _buttonModel.transform.localPosition;

            _targetBlocks.ForEach(t => t.RegisterController(this));
           

            _buttonModel.OnStateChanged += () =>
            {
                if (_buttonModel.State)
                {
                    _buttonModel.transform.localPosition = _initialPosition - new Vector3(0, .1f, 0);
                    _targetBlocks.ForEach(t => t.Color = _color);
                }
                else
                {
                    _buttonModel.transform.localPosition = _initialPosition;
                    _targetBlocks.ForEach(t => t.Color = Color.clear);
                }
            };
        }
    }
}