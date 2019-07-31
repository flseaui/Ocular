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
                /*var pos = _centerParent.position;
                float maxX = pos.x,
                    minX = pos.x,
                    maxY = pos.y,
                    minY = pos.y,
                    maxZ = pos.z,
                    minZ = pos.z;
                _centerParent.ForEachChild(c =>
                {
                    var cPos = c.transform.localPosition;
                    maxX = Mathf.Max(cPos.x, maxX);
                    minX = Mathf.Min(cPos.x, minX);
                    maxY = Mathf.Max(cPos.y, maxY);
                    minY = Mathf.Min(cPos.y, minY);
                    maxZ = Mathf.Max(cPos.z, maxZ);
                    minZ = Mathf.Min(cPos.z, minZ);
                });
    
                /*minX = bounds.min.x;
                maxX = bounds.max.x;
                minY = bounds.min.y;
                maxY = bounds.max.y;
                
                _center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, -10f);*/

                var bounds = _centerParent.GetComponentInChildren<MeshRenderer>().bounds;
                transform.position = new Vector3(bounds.center.x, bounds.center.y + 3, bounds.center.z);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            };
        }
    }
}