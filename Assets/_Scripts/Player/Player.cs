using Game;
using Level;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public enum Cardinal
        {
            North,
            East,
            South,
            West,
            None
        }

        [ShowInInspector] public static bool Falling;

        [ShowInInspector] public static Cardinal Facing;


        public static bool Died;

        public static Action OnDeath;


        private void Awake()
        {
            GameManager.OnLevelLoad += CommitDie;
        }

        private void Update()
        {
            Physics.Raycast(transform.localPosition, Vector3.down, out var hit, 1.5f, LayerMask.GetMask("Model"));
            if (hit.collider == null)
            {
                Falling = true;
                GetComponent<Rigidbody>().AddForce(Vector3.down * 5);
            }
            else
            {
                Falling = false;
            }
        }

        private void OnDestroy()
        {
            GameManager.OnLevelLoad -= CommitDie;
        }

        private void CommitDie()
        {
            Destroy(gameObject);
        }

        public void Death()
        {
            Died = true;
            GetComponent<Rigidbody>().isKinematic = true;
            OnDeath?.Invoke();
        }

        public void ActuallyDie()
        {
            transform.position = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                .PlayerSpawnPoint.position;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Harmful")) Death();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Harmful")) Death();
        }

        public void ChangeFacing(Cardinal newDirection)
        {
            var rot = gameObject.transform.eulerAngles;
            switch (newDirection)
            {
                case Cardinal.North:
                    gameObject.transform.eulerAngles = new Vector3(
                        rot.x,
                        270,
                        rot.z
                    );
                    break;
                case Cardinal.East:
                    gameObject.transform.eulerAngles = new Vector3(
                        rot.x,
                        0,
                        rot.z
                    );
                    break;
                case Cardinal.South:
                    gameObject.transform.eulerAngles = new Vector3(
                        rot.x,
                        90,
                        rot.z
                    );
                    break;
                case Cardinal.West:
                    gameObject.transform.eulerAngles = new Vector3(
                        rot.x,
                        180,
                        rot.z
                    );
                    break;
                case Cardinal.None:
                    break;
                default:
                    return;
            }

            Facing = newDirection;
        }
    }
}
