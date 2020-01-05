#if UNITY_EDITOR

using System.Collections.Generic;
using Game;
using Level;
using Level.Objects;
using OldEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Misc.Tools
{
    public class DisabledObjectDestroyer : OdinEditorWindow
    {
        [MenuItem("Tools/Ocular/Destroy Disabled Objects")]
        private static void OpenWindow()
        {
            GetWindow<DisabledObjectDestroyer>().Show();
        }
        
        public List<GameObject> Levels;

        public bool CheckAllLevels;
        
        [Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        public void DestroyObjects()
        {
            foreach (var level in CheckAllLevels ? GameObject.Find("GameManager").GetComponent<LevelController>().AllWorldLevels : Levels)
            {
                var path = AssetDatabase.GetAssetPath(level);
                var prefab = PrefabUtility.LoadPrefabContents(path);
                
                prefab.transform.Find("MainFloor").ForEachChild(child =>
                {
                    var name = child.name;
                    var model = child.Find("Model");

                    if (!child.gameObject.activeSelf)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                });
                    
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }
    }
}

#endif