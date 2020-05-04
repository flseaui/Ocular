using Game;
using OcularAnimation;
using Player;
using Unity.Mathematics;
using UnityEngine;
using static Player.Player;

namespace Level.Objects.Clones
{
    public class DirectionalClone : MonoBehaviour
    {
        private GameObject _player;
        
        public bool Falling;
        public bool ActuallyFalling;
        public bool Died;
        public bool Invisible;
        public bool Teleporting;

        private bool _superDead = false;

        private float3 _spawnPoint;

        private DirectionalPathfinder _pathfinder;
        
        public float FallingTimer;

        private CloneAnimationController _animController;

        private GameObject _model;
        
        private void Awake()
        {
            _pathfinder = GetComponent<DirectionalPathfinder>();
            _animController = GetComponent<CloneAnimationController>();
            _model = transform.Find("Model").gameObject;
            
            EntityManager.OnEntitiesSpawned += OnEntitiesSpawned;
            LevelController.OnLevelBeginUnload += CommitDie;
            _spawnPoint = transform.position;

            OnDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
           if (!_model.activeSelf)
           {
               gameObject.SetActive(true);
               _animController.enabled = true;
               if (_animController.CurrentGoal != null)
                    _animController.CurrentGoal.transform.parent.gameObject.SetActive(true);
               GameObject.Find("GameManager").GetComponent<GlassesController>().UpdateOcularState();
           }

           transform.position = _spawnPoint;

           Died = false;
           
           if (!Invisible) 
               FakeKillOrRevive(false);
        }
        
        /// <summary>
        /// Fake kill or revive the clone
        /// </summary>
        /// <param name="kill">
        ///    true = kill
        ///    false = revive
        /// </param>
        public void FakeKillOrRevive(bool kill)
        {
            _model.SetActive(!kill);
            transform.Find("DeathTrigger").gameObject.SetActive(!kill);
            _animController.enabled = !kill;
            GetComponent<CapsuleCollider>().enabled = !kill;
        }

        public void GoVisible()
        {
            if (Died) return;
            FakeKillOrRevive(false);
            GetComponent<Rigidbody>().isKinematic = false;
            Invisible = false;
        }

        public void GoInvisible()
        {
            ActuallyDie();
            Invisible = true; 
        }
        
        public void ActuallyDie(bool setDeadAfter = false)
        {
            FakeKillOrRevive(true);
            GetComponent<Rigidbody>().isKinematic = false;

            if (setDeadAfter)
                Died = true;
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
            LevelController.OnLevelBeginUnload -= CommitDie;
            OnDeath -= OnPlayerDeath;
        }

        private void OnEntitiesSpawned()
        {
            _player = GameObject.Find("GameManager").GetComponent<LevelController>().EntityManager.Player;
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
            if (Died || Invisible) return;
            
            if (_player == null)
                _player = GameObject.Find("GameManager").GetComponent<LevelController>().EntityManager.Player;
            
            var clonePath = GetComponent<DirectionalPathfinder>();
            if (Pathfinder.Navigating && (!clonePath.Navigating || clonePath.StopNavNextFrame) && !Falling)
            {
                GetComponent<DirectionalPathfinder>().Step();
            }
            
            var walkable = _pathfinder.GetCurrentWalkable(out _);
            RaycastHit hit;
            if (walkable is SlopeWalkable)
            {
                Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1.5f, LayerMask.GetMask("Model"));
                _pathfinder.OnStairs = true;
                _pathfinder.WalkSpeed = 6.5f;
            }
            else
            {
                Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1f, LayerMask.GetMask("Model"));
                _pathfinder.OnStairs = false;
                _pathfinder.WalkSpeed = 5f;
            }

            if (Teleporting) return;
            
            Physics.Raycast(transform.localPosition, Vector3.down, out var hit2, 1f, LayerMask.GetMask("Model"));
            ActuallyFalling = hit2.collider == null;
            if (hit.collider == null || walkable == null)
            {
                if (!_pathfinder.Navigating)
                {
                    FallingTimer += .2f;
                    Falling = true;
                    GetComponent<Rigidbody>().position += Vector3.down * ((2.5f + FallingTimer) * Time.deltaTime);
                }
            }
            else
            {
                FallingTimer = 0;
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