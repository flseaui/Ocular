using System;
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
            LevelController.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            var levelInfo = GameObject.Find("GameManager").GetComponent<LevelController>().LevelInfo;

            if (levelInfo.HasCustomCamera)
            {
                transform.localPosition = levelInfo.CameraPosition;
                GetComponent<Camera>().orthographicSize = levelInfo.CameraSize;
            }
            else
            {
                _centerParent = levelInfo.transform.Find("Level");

                if (_centerParent == null) return;

                _centerParent = _centerParent.GetChild(0);

                GetComponent<Camera>().orthographicSize = 5;

                var bounds = _centerParent.GetComponentInChildren<MeshRenderer>().bounds;
                transform.position = new Vector3(bounds.center.x, bounds.center.y + 3, bounds.center.z);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10);
            }
        }
    }
}
