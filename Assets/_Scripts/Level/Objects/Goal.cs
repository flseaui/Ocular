using Game;
using Player;
using UnityEngine;

namespace Level.Objects {
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pathfinder.Navigating = false;
                GameManager.OnLevelLoad?.Invoke();
                //ES3.Save<int>("LatestLevel", GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo.Name);
            }
        }
    }
}
