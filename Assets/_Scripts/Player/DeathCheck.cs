using System;
using UnityEngine;

namespace Player
{
    public class DeathCheck : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Harmful"))
                transform.parent.GetComponent<Player>().Death();
        }
    }
}