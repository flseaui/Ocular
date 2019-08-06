﻿using Game;
using UnityEngine;

namespace Level.Objects {
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.OnLevelLoad?.Invoke();
                ES3.Save<int>("LatestLevel", 0);
            }
        }
    }
}
