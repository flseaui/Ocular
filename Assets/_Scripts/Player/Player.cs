using System.ComponentModel;
using System.Runtime.CompilerServices;
using Game;
using Level;
using Level.Objects;
using Misc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        
        private static Colorable _currentCollision;
        [ShowInInspector]
        public static bool Falling;

        [ShowInInspector]public static Cardinal Facing;
        
        public void CheckForDeath(Colorable colorable)
        {
            if (colorable == _currentCollision)
                Death();
        }
        
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
                Falling = false;
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
            transform.position = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                .PlayerSpawnPoint.position;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.ParentHasComponent<Colorable>(out var colorable) && other.transform.parent.CompareTag("Colorable"))
                _currentCollision = colorable;
            if (other.CompareTag("Harmful"))
                Death();
        }

        private void OnTriggerExit(Collider other)
        {
            _currentCollision = null;
        }

        public void ChangeFacing(Cardinal newDirection)
        {
            switch (newDirection)
            {
                case Cardinal.North: gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        270,
                        gameObject.transform.eulerAngles.z
                    );
                    break;
                case Cardinal.East: gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        0,
                        gameObject.transform.eulerAngles.z
                    );
                    break;
                case Cardinal.South: gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        90,
                        gameObject.transform.eulerAngles.z
                    );
                    break;
                case Cardinal.West: gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x,
                        180,
                        gameObject.transform.eulerAngles.z
                    );
                    break;
                default:
                    Debug.Log("Ah fuck");
                    return;
            }
            Facing = newDirection;
        }
        
    }
}