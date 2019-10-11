using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Misc.Tools
{
    public class ShaderSwapper : OdinEditorWindow
    {
        [MenuItem("Tools/Ocular/Shader Swapper")]
        private static void OpenWindow()
        {
            GetWindow<ShaderSwapper>().Show();
        }

        [SerializeField] private List<Material> _materials;
        [SerializeField] private Shader _shader;

        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void SwapShaders()
        {
            foreach (var mat in _materials)
            {
                var tex = mat.mainTexture;
                mat.shader = _shader;
                mat.mainTexture = tex;
                mat.SetTexture("_MainTex", tex);
            }
        }
    }
}