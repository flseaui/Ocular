using System;
using UnityEngine;

namespace LevelEditor
{
    public class ObjectPlacer : MonoBehaviour
    {

        private LevelEditor _levelEditor;
        
        private void Start()
        {
            _levelEditor = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
        }
        
        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var trans = transform;
                var hitPoint = hit.point;
                var normal = hit.normal;
                trans.rotation = Quaternion.FromToRotation(trans.forward, normal) * trans.rotation;
                trans.position = hit.collider.gameObject.transform.position + normal;
     
                
                if (Input.GetMouseButtonDown(0))
                {
                    _levelEditor.PlaceObject(trans.position);
                }
            }
            else
            {
                transform.position = new Vector3(1000, 1000, 1000);
            }
        }
    }
}