using UnityEngine;

namespace OldEditor
{
    public class CustomCursor : MonoBehaviour
    {
        private static Texture2D _texture;

        public float Width, Height;

        private static bool _drawCursor;
        
        public static void SetCursor(Texture2D texture)
        {
            if (texture == null)
            {
                Cursor.visible = true;
                _drawCursor = false;
            }
            else
            {
                _drawCursor = true;
                Cursor.visible = false;
                _texture = texture;
            }
        }

        private void Start()
        {
            Cursor.visible = false;
            _drawCursor = true;
        }

        private void OnGUI()
        {
            if (_drawCursor)
                GUI.DrawTexture(
                    new Rect(Event.current.mousePosition.x - Width / 2, Event.current.mousePosition.y - Height / 2, Width,
                        Height), _texture);
        }
    }
}