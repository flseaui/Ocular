using System;
using Game;
using Misc;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using static Player.Player;

namespace Level.Objects
{
    public class Clone : MonoBehaviour
    {
        private GameObject _player;
        
        public bool Falling;
        public bool ActuallyFalling;
        public bool Died;

        private bool _superDead = false;

        private Vector3 _spawnPoint;
        
        public void ActuallyDie()
        {
            if (_superDead)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = _spawnPoint;
            GetComponent<Rigidbody>().isKinematic = false;
        }
        
        private void Awake()
        {
            EntityManager.OnEntitiesSpawned += OnEntitiesSpawned;
            GameManager.OnLevelLoad += CommitDie;
            _spawnPoint = transform.position;
        }

        private void CommitDie()
        {
            Destroy(gameObject);
        }

        public void Death()
        {
            Died = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        
        private void OnDestroy()
        {
            EntityManager.OnEntitiesSpawned -= OnEntitiesSpawned;
        }

        private void OnEntitiesSpawned()
        {
            _player = GameObject.Find("GameManager").GetComponent<GameManager>().Player;
            //GetComponent<ClonePathfinder>().WalkSpeed = _player.GetComponent<Pathfinder>().WalkSpeed;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Harmful")) Death();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Harmful")) Death();
        }
        
        private void Update()
        {
            if (Pathfinder.Navigating && !GetComponent<ClonePathfinder>().Navigating && !Falling)
            {
                GetComponent<ClonePathfinder>().MirrorClone(_player.GetComponent<Pathfinder>().GetCurrentWalkable(out _), _player.GetComponent<Pathfinder>()._currentEnd);
            }
            
            Physics.Raycast(transform.localPosition, Vector3.down, out var hit, 1.5f, LayerMask.GetMask("Model"));
            Physics.Raycast(transform.localPosition, Vector3.down, out var hit2, 1f, LayerMask.GetMask("Model"));
            //ActuallyFalling = hit2.collider == null;
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