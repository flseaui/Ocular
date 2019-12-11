using Player;
using UnityEngine;

namespace Level.Objects
{
    public class CloneGoal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Clone"))
            {
                other.transform.parent.GetComponent<ClonePathfinder>().Navigating = false;
            }
        }
    }
}