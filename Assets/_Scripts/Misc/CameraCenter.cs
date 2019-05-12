using Level;
using UnityEngine;

namespace Misc
{
    public class CameraCenter : MonoBehaviour
    {
        private Transform _centerParent;

        private Vector3 _center;
        
        private void Start()
        {
            _centerParent = GameObject.Find("GameManager").GetComponent<LevelController>().CurrentLevelInfo.transform
                .GetChild(0);
            var pos = _centerParent.GetChild(0).transform.position;
            float maxX = pos.x,
                minX = pos.x,
                maxY = pos.y,
                minY = pos.y,
                maxZ = pos.z,
                minZ = pos.z;
            _centerParent.ForEachChild(c =>
            {
                var cPos = c.transform.position;
                maxX = Mathf.Max(cPos.x, maxX);
                minX = Mathf.Min(cPos.x, minX);
                maxY = Mathf.Max(cPos.y, maxY);
                minY = Mathf.Min(cPos.y, minY);
                maxZ = Mathf.Max(cPos.z, maxZ);
                minZ = Mathf.Min(cPos.z, minZ);
            });
            
            _center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, (minZ + maxZ) / 2f);
            transform.position = _center;
        }
    }
}