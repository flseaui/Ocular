using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class OnAwake : MonoBehaviour
    {
        public UnityEvent Event;

        private void Awake()
        {
            Event?.Invoke();
        }
    }
}
