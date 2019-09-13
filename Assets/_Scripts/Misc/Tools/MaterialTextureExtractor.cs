using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Misc.Tools
{
    public class MaterialTextureExtractor : OdinEditorWindow
    {
        [MenuItem("Tools/Ocular/Texture Extractor")]
        private static void OpenWindow()
        {
            GetWindow<MaterialTextureExtractor>().Show();
        }

        [SerializeField] private List<Material> _materials;
        [SerializeField] private string _exportPath;
        
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void ExtractTextures()
        {
            var i = 0;
            foreach (var mat in _materials)
            {
                ++i;
                var path = AssetDatabase.GetAssetPath(mat.mainTexture);
                var tex2d = (Texture2D) mat.mainTexture;
                SaveTextureAsPNG(tex2d, $"{_exportPath}/{i}.png");
                //AssetDatabase.CopyAsset(path, $"{_exportPath}/{i}.png");
                //AssetDatabase.CreateAsset(png, $"{_exportPath}/{i}.png)");
            }
        }

        public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
        {
            var _bytes =_texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(_fullPath, _bytes);
            Debug.Log(_bytes.Length/1024  + "Kb was saved as: " + _fullPath);
        }
    }
}