using System;
using System.Collections.Generic;
using cakeslice;
using Level.Objects;
using Misc;
using Sirenix.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LevelEditor
{
    public class ObjectPlacer : MonoBehaviour
    {

        private LevelEditor _levelEditor;
        private SlopeWalkable.Direction _directionFacing;
        private SlopeWalkable.Orientation _orientation;
        private MeshRenderer _renderer;

        private List<GameObject> _selectedObjects;

        [SerializeField] private Texture2D _brushCursor, gearCursor;
        
        public void SetSelectedObjectsColor(Color color)
        {
            if (_selectedObjects.Count < 1) return;
            
            _selectedObjects.ForEach(x => x.transform.parent.GetComponent<Colorable>().Color = color);
        }
        
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _selectedObjects = new List<GameObject>();
            ObjectDrawer.OnObjectSelectionChanged += @object =>
            {
                var filter = GetComponent<MeshFilter>();
                if (@object.transform.GetComponent<MeshFilter>() != null)
                    filter.mesh = @object.transform.GetComponent<MeshFilter>().sharedMesh;
                else
                    filter.mesh = @object.transform.GetComponentsInChildren<MeshFilter>()[0].sharedMesh;
            };
        }
        
        private void Start()
        {
            _levelEditor = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
        }
        
        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetKey(KeyCode.LeftControl))
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.SetCursor(gearCursor, new Vector2(gearCursor.width / 2, gearCursor.height / 2), CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(_brushCursor, new Vector2(_brushCursor.width / 2, _brushCursor.height / 2), CursorMode.ForceSoftware);
            }
            
            if (Physics.Raycast(ray, out var hit))
            {
                var trans = transform;
                var hitPoint = hit.point;
                var normal = hit.normal;
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


                if (Input.GetKey(KeyCode.LeftControl))
                {
                    _renderer.enabled = false;
                    if (Input.GetMouseButtonDown(0))
                    {
                        var outline = hit.collider.gameObject.GetComponent<Outline>();
                        if (outline.enabled)
                        {
                            hit.collider.transform.parent.GetComponentsInChildren<Outline>()
                                .ForEach(x => x.enabled = false);
                            _selectedObjects.Remove(outline.gameObject);
                        }
                        else
                        {
                            hit.collider.transform.parent.GetComponentsInChildren<Outline>()
                                .ForEach(x => x.enabled = true);
                            _selectedObjects.Add(hit.collider.gameObject);
                        }

                    }
                }
                else if (Input.GetKey(KeyCode.LeftAlt))
                {
                    
                }
                else
                {
                    _renderer.enabled = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        _selectedObjects.ForEach(x => x.GetComponent<Outline>().enabled = false);
                        _selectedObjects.Clear();
                        _levelEditor.PlaceObject(trans.position, trans.localRotation);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        foreach (Transform child in hit.collider.transform.parent)
                            Destroy(child.gameObject);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl))
                {
                    _selectedObjects.ForEach(x => x.GetComponent<Outline>().enabled = false);
                    _selectedObjects.Clear();
                }

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