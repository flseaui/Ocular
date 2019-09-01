using Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc
{
    public class CameraCenter : MonoBehaviour
    {
        [ShowInInspector]
        private Transform _centerParent;

        private Vector3 _center;
        
        private void Awake()
        {
            LevelController.OnLevelLoaded += () =>
            {
                _centerParent = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo
                    .transform
                    .Find("Level").GetChild(0);
                

                var bounds = _centerParent.GetComponentInChildren<MeshRenderer>().bounds;
                transform.position = new Vector3(bounds.center.x, bounds.center.y + 3, bounds.center.z);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            };
        }
    }
}