using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Level.Objects
{
    /// <summary>
    /// A button that gets pressed when collided with.
    /// </summary>
    public class Button : MonoBehaviour
    {
        /// <summary>
        /// <c>OnStateChanged</c> is invoked when the buttons state changes.
        /// </summary>
        public Action OnStateChanged;

        private bool _oldState;

        private bool _state;

        private int _cooldown;

        /// <summary>
        /// The state of the button (true - pressed, false - unpressed).
        /// </summary>
        [ShowInInspector, ReadOnly]
        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                if (_state != _oldState)
                {
                    OnStateChanged?.Invoke();
                }

                _oldState = _state;
            }
        }

        private void Start()
        {
            _oldState = _state;
            Player.Player.OnDeath += () => { State = false; };
        }

        private void FixedUpdate()
        {
            if (_cooldown > 0)
            {
                _cooldown--;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_cooldown > 0) return;

            if (!other.transform.CompareTag("Player")) return;

            State = !State;
            _cooldown = 20;
        }

        /*private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if (!State) return;
                State = false;
            }
        }*/
    }
}
