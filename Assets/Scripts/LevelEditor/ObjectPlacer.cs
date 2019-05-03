using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Level.Objects;
using Misc;
using Sirenix.Utilities;
using UnityEngine;

namespace LevelEditor
{
    public class ObjectPlacer : MonoBehaviour
    {
        /// <summary>
        /// Placing mode of the Level Editor placer.
        /// </summary>
        private enum PlacingMode
        {
            Place,
            Select,
            Customize
        }

        /// <summary>
        /// This placers current placing mode.
        /// </summary>
        private PlacingMode _mode;
        /// <summary>
        /// If true the placing mode was changed last frame.
        /// </summary>
        private bool _changedMode = true;

        private Orientation _orientation;
        private Direction _direction;
        
        private GameObject _meshes;
        private List<GameObject> _selectedObjects;

        private LevelEditor _levelEditor;
        
        [SerializeField] private Texture2D 
            _selectCursor,
            _brushCursor,
            _gearCursor;

        [SerializeField] private Material _placerMat;

        public void SetSelectedObjectsColor(Color color)
        {
            if (_selectedObjects.Count < 1) return;

            _selectedObjects.ForEach(x => x.transform.parent.GetComponent<Colorable>().Color = color);
        }
        
        private void Update()
        {
            var shift = Input.GetKey(KeyCode.LeftShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl);
            var alt = Input.GetKey(KeyCode.LeftAlt);

            var left = Input.GetMouseButtonDown(0);
            var right = Input.GetMouseButtonDown(1);
            
            var hObj = GetHighlightedObject(out var normal);
            if (hObj != null)
            {
                transform.position = hObj.transform.position + normal; 
                
                if      (_direction == Direction.Right   ) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 0  , 0);
                else if (_direction == Direction.Left    ) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
                else if (_direction == Direction.Forward ) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
                else if (_direction == Direction.Back    ) transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, 90 , 0);
                if      (_orientation == Orientation.Up  ) transform.localRotation = Quaternion.Euler(0  , transform.localEulerAngles.y, 0);
                else if (_orientation == Orientation.Down) transform.localRotation = Quaternion.Euler(180, transform.localEulerAngles.y, 0);
            }
            else
            {
                transform.position = new Vector3(1000, 1000, 1000);
            }

            var currentMode = _mode;
            
            switch (_mode)
            {
                case PlacingMode.Place:
                    if (_changedMode)
                    {
                        SetVisibility(true);
                        CustomCursor.SetCursor(_brushCursor);
                    }

                    if (left)
                    {
                        _levelEditor.PlaceObject(transform.position, transform.rotation);
                    }

                    if (right)
                    {
                        Destroy(hObj);
                    }
                    
                    if (ctrl)
                        _mode = PlacingMode.Select;
                    else if (alt)
                        _mode = PlacingMode.Customize;
                    break;

                case PlacingMode.Select:
                    if (_changedMode)
                    {
                        SetVisibility(false);
                        CustomCursor.SetCursor(_selectCursor);
                    }
                    
                    if (!ctrl)
                        _mode = PlacingMode.Place;
                    break;
                
                case PlacingMode.Customize:
                    if (_changedMode)
                    {
                        SetVisibility(false);
                        CustomCursor.SetCursor(_gearCursor);
                    }
                    
                    if (!alt)
                        _mode = PlacingMode.Place;
                    break;
            }

            if (currentMode != _mode)
                _changedMode = true;
        }
        
        /// /// <summary>
        /// Gets the object highlighted by the placer.
        /// </summary>
        /// <param name="normal">
        /// The normal of the raycast hit between the placer and the object.</param>
        /// <returns>
        /// The currently highlighted GameObject.
        /// </returns>
        [CanBeNull]
        private GameObject GetHighlightedObject(out Vector3 normal)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                normal = hit.normal;
                return hit.collider.gameObject;
            }
            
            normal = Vector3.zero;
            return null;
        }
        
        /// <summary>
        /// Set visibility of selected block shadow.
        /// </summary>
        /// <param name="visibility"></param>
        private void SetVisibility(bool visibility)
        {
            _meshes.SetActive(visibility);
        }
        
        private void Awake()
        {
            ObjectDrawer.OnObjectSelectionChanged += OnObjectChanged;
            
            _selectedObjects = new List<GameObject>();

            _meshes = transform.GetChild(0).gameObject;
            _levelEditor = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
        }

        /// <summary>
        /// Updates the placer mesh to reflect the selected object.
        /// </summary>
        /// <param name="object">
        /// The newly selected object.
        /// </param>
        private void OnObjectChanged(GameObject @object)
        {
            _meshes.transform.ForEachChild(x => Destroy(x.gameObject));

            void CreateMesh(GameObject mesh)
            {
                var obj = new GameObject("Mesh");
                obj.transform.parent = _meshes.transform;
                obj.transform.localPosition = mesh.transform.localPosition;
                obj.transform.localScale = mesh.transform.localScale;
                obj.transform.localRotation = mesh.transform.localRotation;
                var filter = obj.AddComponent<MeshFilter>();
                filter.mesh = mesh.GetComponent<MeshFilter>().sharedMesh;
                var renderer = obj.AddComponent<MeshRenderer>();
                renderer.material = _placerMat;
            }
            
            // If mesh is directly on object
            if (@object.transform.GetComponent<MeshFilter>() != null)
                CreateMesh(@object);
            else
                @object.transform.ForEachChild(x => CreateMesh(x.gameObject));
        }
    }
}