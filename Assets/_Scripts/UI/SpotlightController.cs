using System;
using UnityEngine;

namespace UI
{
    public class SpotlightController : MonoBehaviour
    {
        public static Action SpotlightEnabled;

        [SerializeField] private GameObject _realSpotlight;

        private void OnDisable()
        {
            _realSpotlight.SetActive(false);
        }

        public void EnableWheel()
        {
            Debug.Log("TESUTOSETOSIETOSIEJTOISEJTOSIEJTOSIEJTOSIEJTOSIEJ");
            SpotlightEnabled?.Invoke();
            gameObject.SetActive(false);
            _realSpotlight.SetActive(true);
        }
    }
}