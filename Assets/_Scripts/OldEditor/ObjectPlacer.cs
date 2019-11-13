#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Game;
using JetBrains.Annotations;
using Level.Objects;
using Misc;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Outline = cakeslice.Outline;
using OcularState = Game.GlassesController.OcularState;

namespace OldEditor
{
    /// <summary>
    /// Used to place and interact with objects in the level editor, works on a state based system.
    /// </summary>
    public class ObjectPlacer : MonoBehaviour
    {
        /// <summary>
        /// Placing mode of the Level Editor placer.
        /// </summary>
        private enum PlacingMode
        {
            Place,
            Select,
            Customize,
            SelectButtonTargets
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
        private List<Outline> _tempOutlines;

        private GameObject _customizingObject;
        
        private LevelEditor _levelEditor;
        private GraphicRaycaster _graphicRaycaster;
        private PointerEventData _pointer;
        
        [SerializeField] private Texture2D 
            _selectCursor,
            _brushCursor,
            _gearCursor;

        [SerializeField] private Material _placerMat;
        [SerializeField] private GameObject _buttonColorPopupPrefab;

        private GameObject _popup;
        
        /// <summary>
        /// Colors all currently selected objects.
        /// </summary>
        /// <param name="color">
        /// The color the objects will be colored.
        /// </param>
        public void SetSelectedObjectsColor(OcularState color)
        {
            if (_selectedObjects.Count < 1) return;

            _selectedObjects.ForEach(x => x.transform.parent.GetComponent<Colorable>().OcularState = color);
        }
        
        private void Update()
        {
            var shift = Input.GetKey(KeyCode.LeftShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl);
            var alt = Input.GetKey(KeyCode.LeftAlt);

            var left = Input.GetMouseButtonDown(0);
            var right = Input.GetMouseButtonDown(1);

            _pointer.position = Input.mousePosition;
            
            var hObj = GetHighlightedObject(out var normal);
            var hUI = GetUIUnderMouse();
            var validObject = hObj != null;
            var isUI = hUI != null;
            if (validObject)
            {
                var side = Vector3.zero;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("EditorTrigger")))
                {
                    var center = hit.collider.bounds.center;
                    side.x = Math.Abs(hit.point.x - center.x) >= .5 ? hit.point.x > center.x ? 1 : -1 : 0;
                    side.y = Math.Abs(hit.point.y - center.y) >= .5 ? hit.point.y > center.y ? 1 : -1 : 0;
                    side.z = Math.Abs(hit.point.z - center.z) >= .5 ? hit.point.z > center.z ? 1 : -1 : 0;
                }

                transform.position = hObj.transform.position + side;
                
                if      (_direction == Direction.Right   ) transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, 0  , 0);
                else if (_direction == Direction.Left    ) transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, 180, 0);
                else if (_direction == Direction.Forward ) transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, 270, 0);
                else if (_direction == Direction.Back    ) transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, 90 , 0);
                if      (_orientation == Orientation.Up  ) transform.rotation = Quaternion.Euler(0  , transform.localEulerAngles.y, 0);
                else if (_orientation == Orientation.Down) transform.rotation = Quaternion.Euler(180, transform.localEulerAngles.y, 0);
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
                    
                    _tempOutlines.ForEach(x => x.enabled = false);
                    
                    if (left)
                    {
                        _selectedObjects.ForEach(x => x.GetComponent<Outline>().enabled = false);
                        _selectedObjects.Clear();
                        if (_popup != null)
                            Destroy(_popup.gameObject);
                    }
                    
                    if (validObject)
                    {
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            if (shift)
                            {
                                _orientation = _orientation == Orientation.Down
                                    ? Orientation.Up
                                    : Orientation.Down;
                            }
                            else
                            {
                                var dir = (int) _direction;
                                if (dir++ > 2)
                                    dir = 0;
                                _direction = (Direction) dir;
                            }
                        }
                        
                        if (left)
                        {
                            var gridPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                            _levelEditor.PlaceElement(gridPosition, _orientation, _direction);
                        }

                        if (right)
                        {
                            //hObj.transform.parent.ForEachChild(x => Destroy(x.gameObject));
                            Destroy(hObj.transform.parent.gameObject);
                        }
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

                    _tempOutlines.ForEach(x => x.enabled = false);
                    
                    if (validObject)
                    {
                        if (left)
                        {
                            var outline = hObj.GetComponent<Outline>();
                            
                            if (outline.enabled)
                                _selectedObjects.Remove(outline.gameObject);
                            else
                                _selectedObjects.Add(hObj);
                            
                            hObj.transform.parent.GetComponentsInChildren<Outline>()
                                .ForEach(x => x.enabled = !outline.enabled);
                        }
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

                    _tempOutlines.ForEach(x => x.enabled = false);
                    
                    if (validObject)
                    {
                        if (hObj.transform.ParentHasComponent<ButtonWalkable>(out var button))
                        {
                            button.TargetBlocks.ForEach(x => x.gameObject.GetComponentsInChildren<Outline>().ForEach(o =>
                            {
                                o.enabled = true;
                                _tempOutlines.Add(o);
                            }));
                        }
                        if (left)
                        {
                            if (_popup != null)
                            {
                                Destroy(_popup.gameObject);
                            }

                            if (hObj.transform.ParentHasComponent(out button))
                            {
                                _popup = Instantiate(_buttonColorPopupPrefab, hObj.transform.parent.position,
                                    Quaternion.identity, GameObject.Find("Canvas").transform);
                                var popup = _popup.GetComponent<ButtonColorPopup>();
                                popup.Target = hObj.transform.parent;
                                popup.GetComponent<ColorPalette>().OnColorChanged = color =>
                                {
                                    button.Color = color;
                                };

                                _customizingObject = button.gameObject;
                                _mode = PlacingMode.SelectButtonTargets;
                                break;
                            }
                        }
                    }
                    
                    if (!alt)
                        _mode = PlacingMode.Place;
                    break;
                case PlacingMode.SelectButtonTargets:
                    if (_changedMode)
                    {
                        SetVisibility(false);
                        CustomCursor.SetCursor(_selectCursor);
                    }
                    
                    if (validObject)
                    {
                        if (left && !isUI)
                        {
                            var button = _customizingObject.GetComponent<ButtonWalkable>();
                            var outline = hObj.GetComponent<Outline>();
                            
                            if (outline.enabled)
                                button.TargetBlocks.Remove(hObj.GetComponentInParent<Colorable>());
                            else
                                button.TargetBlocks.Add(hObj.GetComponentInParent<Colorable>());
                            
                            hObj.transform.parent.GetComponentsInChildren<Outline>()
                                .ForEach(x => x.enabled = !outline.enabled);
                        }
                    }
                    else
                    {
                        if (left && !isUI)
                        {
                            _mode = PlacingMode.Place;

                            _customizingObject.GetComponent<ButtonWalkable>().TargetBlocks
                                .ForEach(x => x.GetComponentsInChildren<Outline>()
                                    .ForEach(o => o.enabled = false));
                            
                            if (_popup != null)
                            {
                                Destroy(_popup.gameObject);
                            }
                        }
                    }
                    
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
            if (Physics.Raycast(ray, out var hit,Mathf.Infinity, LayerMask.GetMask("Model")))
            {
                normal = hit.normal;
                return hit.collider.gameObject;
            }
            
            normal = Vector3.zero;
            return null;
        }

        [CanBeNull]
        private GameObject GetUIUnderMouse()
        {
            var results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(_pointer, results);

            return results.Count > 0 ? results[0].gameObject : null;
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
            LevelEditor.OnLevelPlayToggle += OnLevelPlayToggle;
            
            _selectedObjects = new List<GameObject>();
            _tempOutlines = new List<Outline>();
            
            _meshes = transform.GetChild(0).gameObject;
            _levelEditor = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
            var canvas = GameObject.Find("Canvas");
            _pointer = new PointerEventData(canvas.GetComponent<EventSystem>());
            _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        }

        private void OnDestroy()
        {
            ObjectDrawer.OnObjectSelectionChanged -= OnObjectChanged;
            LevelEditor.OnLevelPlayToggle -= OnLevelPlayToggle;
            
            CustomCursor.SetCursor(null);
        }

        private void OnLevelPlayToggle(bool state)
        {
            gameObject.SetActive(!state);
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
                @object.transform.ForEachChild(x =>
                {
                    if (!x.CompareTag("EditorTrigger")) CreateMesh(x.gameObject);
                });
        }
    }
}

#endif