using System;
using Level.Objects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditor
{
    public class ObjectPlacer : MonoBehaviour
    {

        private LevelEditor _levelEditor;
        private SlopeWalkable.Direction _directionFacing;
        private SlopeWalkable.Orientation _orientation;

        private void Awake()
        {
            ObjectDrawer.OnObjectSelectionChanged += @object =>
            {
                GetComponent<MeshFilter>().mesh = @object.transform.Find("Model").GetComponent<MeshFilter>().sharedMesh;
            };
        }
        
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
                //trans.rotation = Quaternion.FromToRotation(trans.forward, normal) * trans.rotation;
                if (_directionFacing == SlopeWalkable.Direction.Right)
                    transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 0, 0);
                else if (_directionFacing == SlopeWalkable.Direction.Left)
                    transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
                else if (_directionFacing == SlopeWalkable.Direction.Forward)
                    transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
                else if (_directionFacing == SlopeWalkable.Direction.Back)
                    transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 90, 0);

                if (_orientation == SlopeWalkable.Orientation.Up)
                    transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
                if (_orientation == SlopeWalkable.Orientation.Down)
                    transform.localRotation = Quaternion.Euler(180, transform.localEulerAngles.y, 0);
                trans.position = hit.collider.gameObject.transform.position + normal;
     
                
                if (Input.GetMouseButtonDown(0))
                {
                    _levelEditor.PlaceObject(trans.position, trans.localRotation);
                }
            }
            else
            {
                transform.position = new Vector3(1000, 1000, 1000);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _orientation = _orientation == SlopeWalkable.Orientation.Down
                        ? SlopeWalkable.Orientation.Up
                        : SlopeWalkable.Orientation.Down;
                }
                else
                {
                    var dir = (int) _directionFacing;
                    if (dir++ > 2)
                        dir = 0;
                    _directionFacing = (SlopeWalkable.Direction) dir;
                }
            }
        }
    }
}