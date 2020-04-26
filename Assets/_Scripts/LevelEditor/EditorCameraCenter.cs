using Level;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Mathematics;

namespace LevelEditor
{
    public class EditorCameraCenter : MonoBehaviour
    {
        [ShowInInspector]
        private Transform _centerParent;

        private float3 _center;

        public LevelInfo LevelInfo;

        public void Init()
        {
            if (LevelInfo.HasCustomCamera)
            {
                transform.localPosition = LevelInfo.CameraPosition;
                GetComponent<Camera>().orthographicSize = LevelInfo.CameraSize;
            }
            else
            {
                _centerParent = LevelInfo.transform.Find("Level");

                if (_centerParent == null)
                    return;

                _centerParent = _centerParent.GetChild(0);

                GetComponent<Camera>().orthographicSize = 5;

                var bounds = _centerParent.GetComponentInChildren<MeshRenderer>().bounds;
                transform.position = new Vector3(bounds.center.x, bounds.center.y + 3, bounds.center.z);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            }
        }
    }
}
