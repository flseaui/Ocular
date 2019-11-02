#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using OcularAnimation;

public class VoxelAnimationCreator : OdinEditorWindow
{
    [MenuItem("Tools/Ocular/Voxel Animation Creator")]
    private static void OpenWindow()
    {
        GetWindow<VoxelAnimationCreator>().Show();
    }

    [SerializeField]
    private List<GameObject> _frameObjects;

    [SerializeField]
    private string _name;

    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    public void CreateAnimation()
    {
        var anim = ScriptableObject.CreateInstance<VoxelAnimation>();

        anim.Frames = new List<AnimFrame>();

        foreach (var frame in _frameObjects)
        {
            var model = frame.transform.GetChild(0);
            var mat = model.GetComponent<Renderer>().sharedMaterial;
            var mesh = model.GetComponent<MeshFilter>().sharedMesh;
            anim.Frames.Add(new AnimFrame(mesh, mat, 10000));
        }

        AssetDatabase.CreateAsset(anim, $"Assets/{_name}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = anim;
    }
}
#endif
