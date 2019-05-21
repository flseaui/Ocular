using System;
using UnityEngine;

namespace Player
{
    public class DeathCheck : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            transform.parent.GetComponent<Player>().Death();
        }
    }
}