using System;
using UnityEngine;

namespace UI
{
    public class SpotlightController : MonoBehaviour
    {
        public static Action SpotlightEnabled;
        
        public void EnableWheel()
        {
            SpotlightEnabled?.Invoke();
        }
    }
}