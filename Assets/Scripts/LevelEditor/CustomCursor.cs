using System;
using UnityEngine;

namespace LevelEditor
{
    public class CustomCursor : MonoBehaviour
    {
        private static Texture2D _texture;

        public float Width, Height;

        public static void SetCursor(Texture2D texture) => _texture = texture;

        private void Start()
        {
            Cursor.visible = false;
        }

        private void OnGUI()
        {
            GUI.DrawTexture(
                new Rect(Event.current.mousePosition.x - Width / 2, Event.current.mousePosition.y - Height / 2, Width,
                    Height), _texture);
        }
    }
}