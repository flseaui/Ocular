using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Objects {
    public class Button : MonoBehaviour
    {
        public Action OnStateChanged;

        [ShowInInspector, ReadOnly] public bool State;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if (State) return;
                State = true;
                OnStateChanged?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if (!State) return;
                State = false;
                OnStateChanged?.Invoke();
            }
        }
    }
}