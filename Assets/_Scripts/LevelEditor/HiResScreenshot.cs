using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace LevelEditor
{
    public class HiResScreenshot : MonoBehaviour {
        public int resWidth = 1920; 
        public int resHeight = 1080;
 
        private static bool _takeHiResShot;

        private static string _name;
        
        public static string ScreenShotName(int width, int height) {
            return
                $"Assets/_Prefabs/Levels/Thumbnails/thumb_{_name}.png";
        }
 
        public static void TakeHiResShot(string name)
        {
            _name = name;
            _takeHiResShot = true;
        }

        private void LateUpdate() {
            _takeHiResShot |= Input.GetKeyDown("k");
            if (_takeHiResShot) {
                var rt = new RenderTexture(resWidth, resHeight, 24);
                Camera.main.targetTexture = rt;
                var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
                Camera.main.targetTexture = rt;
                RenderTexture.active = rt; 
                Camera.main.Render();
                screenShot.ReadPixels( new Rect(0, 0, resWidth , resHeight ), 0, 0 );
                screenShot.Apply(false);
                RenderTexture.active = null;
                Camera.main.targetTexture = null;
                Destroy(rt);
                var bytes = screenShot.EncodeToPNG();
                var filename = ScreenShotName(resWidth, resHeight);
                using (var fsStream = new FileStream(filename, FileMode.Create))
                using(var bw = new BinaryWriter(fsStream, Encoding.Default))
                {
                    bw.Write(bytes);
                    bw.Flush();
                    bw.Close();
                }
                AssetDatabase.Refresh();
                //System.IO.File.WriteAllBytes(filename, bytes);
                Debug.Log($"Took screenshot to: {filename}");
                _takeHiResShot = false;
            }
        }
    }
}

#endif