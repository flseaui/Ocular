using System.Collections.Generic;
using Game;
using Level.Objects;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Misc.Tools
{
    public class LevelModelConverter : OdinEditorWindow
    {
        [MenuItem("Tools/Ocular/Convert Levels")]
        private static void OpenWindow()
        {
            GetWindow<LevelModelConverter>().Show();
        }


        public GameObject Spawn;
        public GameObject Goal;
        
        public List<GameObject> BlockVariants;
        public List<GameObject> StairVariants;
        
        public List<GameObject> Levels;
        
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void ConvertLevels()
        {
            foreach (var level in Levels)
            {
                level.transform.Find("MainFloor").ForEachChild(child =>
                {
                    var name = child.name;
                    var model = child.Find("Model");
                    if (name.StartsWith("Floor"))
                    {
                        if (child.GetComponent<Colorable>().OcularState == GlassesController.OcularState.Z)
                        {
                            var spawn = Spawn.transform.GetChild(0);
                        
                            model.GetComponent<MeshFilter>().sharedMesh = spawn.GetComponent<MeshFilter>().sharedMesh;
                            model.GetComponent<MeshRenderer>().sharedMaterials =
                                spawn.GetComponent<MeshRenderer>().sharedMaterials;
                            
                            model.transform.localScale = spawn.transform.localScale; 
                            child.transform.localScale = Spawn.transform.localScale;
                        }
                        else
                        {
                            var block = BlockVariants[Random.Range(0, BlockVariants.Count)]
                                .transform.GetChild(0);

                            model.GetComponent<MeshFilter>().sharedMesh = block.GetComponent<MeshFilter>().sharedMesh;
                            model.GetComponent<MeshRenderer>().sharedMaterials =
                                block.GetComponent<MeshRenderer>().sharedMaterials;
                            model.transform.localScale = block.transform.localScale;
                            child.transform.localScale = block.parent.localScale;
                        }
                    }
                    else if (name.StartsWith("Goal"))
                    {
                        var goal = Goal.transform.GetChild(0);
                        
                        model.GetComponent<MeshFilter>().sharedMesh = goal.GetComponent<MeshFilter>().sharedMesh;
                        model.GetComponent<MeshRenderer>().sharedMaterials = goal.GetComponent<MeshRenderer>().sharedMaterials;
                        model.transform.localScale = goal.transform.localScale;
                        child.transform.localScale = Goal.transform.localScale;
                    }
                    else if (name.StartsWith("Stair"))
                    {
                        var stair = StairVariants[Random.Range(0, StairVariants.Count)]
                            .transform.GetChild(0);

                        model.GetComponent<MeshFilter>().sharedMesh = stair.GetComponent<MeshFilter>().sharedMesh;
                        model.GetComponent<MeshRenderer>().sharedMaterials = stair.GetComponent<MeshRenderer>().sharedMaterials;
                        model.transform.localScale = stair.transform.localScale;
                        child.transform.localScale = stair.parent.localScale;
                    }
                });

                PrefabUtility.SavePrefabAsset(level);
            }
        }
    }
}