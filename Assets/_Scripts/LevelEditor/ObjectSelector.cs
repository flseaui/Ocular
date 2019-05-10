using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor
{
    public class ObjectSelector : MonoBehaviour, IPointerDownHandler
    {
        private ObjectDrawer _objectDrawer;
        public GameObject Object;
        
        public void Deselect()
        {
            GetComponent<Image>().color = Color.white;
        }

        public void Select()
        {
            GetComponent<Image>().color = Color.yellow;
            _objectDrawer.SetObject(Object);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
        }

        private void Awake()
        {
            _objectDrawer = GetComponentInParent<ObjectDrawer>();
        }
        
    }
}