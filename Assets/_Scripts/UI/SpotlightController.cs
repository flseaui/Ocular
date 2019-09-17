using System;
using UnityEngine;

namespace UI
{
    public class SpotlightController : MonoBehaviour
    {
        public static Action SpotlightEnabled;

        [SerializeField] private GameObject _realSpotlight;
        
        public void EnableWheel()
        {
            SpotlightEnabled?.Invoke();
            _realSpotlight.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}