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
                var levelInfo = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo;

                if (levelInfo.HasCustomCamera)
                {
                    transform.position = levelInfo.CameraPosition;
                    GetComponent<Camera>().orthographicSize = levelInfo.CameraSize;
                }
                else
                {
                    _centerParent = levelInfo.transform.Find("Level");
    
                    if (_centerParent == null)
                        return;
                    
                    _centerParent = _centerParent.GetChild(0);
    
                    var bounds = _centerParent.GetComponentInChildren<MeshRenderer>().bounds;
                    transform.position = new Vector3(bounds.center.x, bounds.center.y + 3, bounds.center.z);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
                }
            };
        }
    }
}