using UnityEngine;
using Battlehub.UIControls.MenuControl;

namespace Battlehub.RTEditor
{
    public interface IContextMenu
    {
        void Open(MenuItemInfo[] items);
    }

    public class ContextMenu : MonoBehaviour, IContextMenu
    {
        [SerializeField]
        private Menu m_menu = null;

        [SerializeField]
        private RectTransform m_contextMenuArea = null;

        public void Open(MenuItemInfo[] items)
        {
            Canvas canvas = m_contextMenuArea.GetComponentInParent<Canvas>();
            Vector3 position;
            Vector2 pos = Input.mousePosition;

            if (!RectTransformUtility.RectangleContainsScreenPoint(m_contextMenuArea, pos, canvas.worldCamera))
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_contextMenuArea, pos, canvas.worldCamera, out position))
            {
                m_menu.transform.position = position;
                m_menu.Items = items;
                m_menu.Open();
            }
        }
    }
}

