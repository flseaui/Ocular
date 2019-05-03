using System.Collections.Generic;
using System.Linq;
using Misc;
using Sirenix.Utilities;
using UnityEngine;

namespace LevelEditor
{
    public class BetterObjectPlacer : MonoBehaviour
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

        private PlacingMode _mode;

        private GameObject _meshes;
        private List<GameObject> _selectedObjects;

        [SerializeField] private Texture2D _selectCursor, _brushCursor, _gearCursor;
        
        private void Update()
        {
            var shift = Input.GetKey(KeyCode.LeftShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl);
            var alt = Input.GetKey(KeyCode.LeftAlt);

            switch (_mode)
            {
                case PlacingMode.Place:
                    if (ctrl)
                        _mode = PlacingMode.Select;
                    else if (alt)
                        _mode = PlacingMode.Customize;
                    
                    CustomCursor.SetCursor(_brushCursor);
                    
                    break;

                case PlacingMode.Select:
                    if (!ctrl)
                        _mode = PlacingMode.Place;
                    
                    CustomCursor.SetCursor(_selectCursor);
                    
                    break;
                
                case PlacingMode.Customize:
                    if (!alt)
                        _mode = PlacingMode.Place;
                    
                    CustomCursor.SetCursor(_gearCursor);
                    
                    break;
            }
        }
        
        private void Awake()
        {
            ObjectDrawer.OnObjectSelectionChanged += OnObjectChanged;
            
            _selectedObjects = new List<GameObject>();

            _meshes = transform.GetChild(0).gameObject;
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

            // If mesh is directly on object
            if (@object.transform.GetComponent<MeshFilter>() != null)
                Instantiate(@object.transform.GetComponent<MeshFilter>().sharedMesh, _meshes.transform);
            else
                @object.transform.GetComponentsInChildren<MeshFilter>()
                    .ForEach(x => Instantiate(x, _meshes.transform));
        }
    }
}