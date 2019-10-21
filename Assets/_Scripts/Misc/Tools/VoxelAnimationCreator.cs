using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class VoxelAnimationCreator : OdinEditorWindow {
    [MenuItem("Tools/Ocular/Voxel Animation Creator")]
    private static void OpenWindow()
    {
        GetWindow<VoxelAnimationCreator>().Show();
    }

    [SerializeField]
    private List<GameObject> 

    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    public void CreateAnimation() {

    }
}