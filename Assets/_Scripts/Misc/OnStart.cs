using UnityEngine;
using UnityEngine.Events;

namespace Misc
{
    public class OnStart : MonoBehaviour
    {
        public UnityEvent Event;

        private void Start()
        {
            Event?.Invoke();
        }
    }
}
