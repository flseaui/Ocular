using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace OldEditor
{
    public class ObjectDrawer : MonoBehaviour
    {
        public GameObject SelectedObject;

        private ObjectSelector[] _objectSelectors;

        public static Action<GameObject> OnObjectSelectionChanged;
        
        private void Awake()
        {
            _objectSelectors = GetComponentsInChildren<ObjectSelector>();
        }

        private void Start()
        {
            _objectSelectors[0].Select();
        }
        
        public void SetObject(GameObject @object)
        {
            SelectedObject = @object;
            _objectSelectors.Where(x => x.Object != SelectedObject).ForEach(x => x.Deselect());
            OnObjectSelectionChanged?.Invoke(SelectedObject);

        }
        

    }
}