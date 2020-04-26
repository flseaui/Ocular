using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Objects
{
    /// <summary>
    /// A walkable object housing a button, can target multiple blocks and change their color on press.
    /// </summary>
    public class ButtonWalkable : Walkable, IController
    {
        [SerializeField] private Button _buttonModel;

        [SerializeField] public GlassesController.OcularState Color;

        private Vector3 _initialPosition;

        /// <summary>
        /// The blocks that will be modified when the button is pressed.
        /// </summary>
        public List<Colorable> TargetBlocks;

        /// <summary>
        /// The state of the <c>Button</c> (true - pressed, false - unpressed).
        /// </summary>
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

            // register with all target blocks
            TargetBlocks.ForEach(t => t.RegisterController(this));

            _buttonModel.OnStateChanged += OnStateChanged;
        }

        private void OnStateChanged()
        {
            if (_buttonModel.State)
            {
                // Press button
                _buttonModel.transform.localPosition = _initialPosition - new Vector3(0, .1f, 0);
                TargetBlocks.ForEach(t =>
                {
                    t.OcularColor = Color;
                    t.UpdateState();
                });
            }
            else
            {
                // Release button
                _buttonModel.transform.localPosition = _initialPosition;
                TargetBlocks.ForEach(t =>
                {
                    t.OcularColor = GlassesController.OcularState.Null;
                    t.UpdateState();
                });
            }
        }

        private void OnDestroy()
        {
            _buttonModel.OnStateChanged -= OnStateChanged;
        }
    }
}
