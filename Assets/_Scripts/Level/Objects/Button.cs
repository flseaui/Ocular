using System;
using Sirenix.OdinInspector;
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
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                /*if (State) return;*/
                State = !State;
            }
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