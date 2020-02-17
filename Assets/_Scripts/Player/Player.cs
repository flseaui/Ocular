using Game;
using Level;
using Sirenix.OdinInspector;
using System;
using Level.Objects;
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
        [ShowInInspector] public static bool ActuallyFalling;

        [ShowInInspector] public static Cardinal Facing;


        public static bool Died;

        public static Action OnDeath;

        private bool _superDead = false;

        private Pathfinder _pathfinder;

        public static float FallingTimer;

        public static bool Teleporting;
        
        private void Awake()
        {
            _pathfinder = GetComponent<Pathfinder>();
            GameManager.OnLevelLoad += CommitDie;
        }

        private void Update()
        {
            if (Teleporting) return;
            
            var walkable = _pathfinder.GetCurrentWalkable(out _);
            RaycastHit hit;
            if (walkable is SlopeWalkable)
            {
                Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1.5f, LayerMask.GetMask("Model"));
                Pathfinder.OnStairs = true;
                _pathfinder.WalkSpeed = 6.5f;
            }
            else
            {
                Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1f, LayerMask.GetMask("Model"));
                Pathfinder.OnStairs = false;
                _pathfinder.WalkSpeed = 5f;
            }

            Physics.Raycast(transform.localPosition, Vector3.down, out var hit2, 1f, LayerMask.GetMask("Model"));
            ActuallyFalling = hit2.collider == null;
            if (hit.collider == null || walkable == null)
            {
                FallingTimer += .2f;
                Falling = true;
                Debug.Log("FREE FALLIN");
                GetComponent<Rigidbody>().position += Vector3.down * ((2.5f + FallingTimer) * Time.deltaTime);
            }
            else
            {
                FallingTimer = 0;
                Falling = false;
            }
        }

        private void OnDestroy()
        {
            GameManager.OnLevelLoad -= CommitDie;
        }

        public void CommitDie()
        {
            Destroy(gameObject);
        }

        public void Death()
        {
            Died = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        // Kill with no respawn
        public void SuperKill()
        {
            _superDead = true;
            Died = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        public void ActuallyDie()
        {
            if (_superDead)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                .PlayerSpawnPoint.position;
            GetComponent<Rigidbody>().isKinematic = false;
            
            OnDeath?.Invoke();
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
