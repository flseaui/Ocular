using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class GameEventListener : MonoBehaviour
    {
        [Serializable]
        private struct EventAndResponse
        {
            public GameEvent Event;
            public UnityEvent Response;   
        }

        [SerializeField] private List<EventAndResponse> _events = new List<EventAndResponse>();

        private void OnEnable()
        {
            foreach (var @event in _events)
            {
                @event.Event.RegisterListener(this);
            }
        }
        
        private void OnDisable()
        {
            foreach (var @event in _events)
            {
                @event.Event.UnregisterListener(this);
            }
        }

        public void OnEventRaised(GameEvent @event)
        {
            _events.Find(x => x.Event == @event).Response?.Invoke();
        }
    }
}