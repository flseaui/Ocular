#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using OcularAnimation;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Misc.Tools
{
    public class ShaderReplacer : OdinEditorWindow
    {
        [MenuItem("Tools/Ocular/Shader Replacer")]
        private static void OpenWindow()
        {
            GetWindow<ShaderReplacer>().Show();
        }

        [SerializeField] private List<NewVoxelAnimation> _anims;
        [SerializeField] private Shader _newShader;
        
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void ReplaceShaders()
        {
            foreach (var anim in _anims)
            {
                var frames = (List<VoxelFrame>) typeof(NewVoxelAnimation).GetField("_frames", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(anim);

                if (frames == null)
                {
                    Debug.Log($"Anim {anim.name} was empty!");
                    continue;
                }
                
                foreach (var frame in frames)
                {
                    foreach (var mesh in frame.Meshes)
                    {
                        mesh.GetComponent<Renderer>().sharedMaterial.shader = _newShader;
                    }
                }
            }
        }
    }
}

#endif