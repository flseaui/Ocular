using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Objects 
{
    public class Button : MonoBehaviour
    {
        public Action OnStateChanged;

        private bool _oldState;

        private bool _state;
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
                if (State) return;
                State = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if (!State) return;
                State = false;
            }
        }
    }
}