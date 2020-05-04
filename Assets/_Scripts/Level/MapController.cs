using System.Collections;
using System.Linq;
using Boo.Lang;
using Level.Objects;
using Level.Objects.Clones;
using Misc;
using UnityEngine;

namespace Level {
    public class MapController : MonoBehaviour
    {
        public static int NUM_WALKABLES;

        private Walkable[] _walkables;
        private Colorable[] _colorables;
        private GameObject[] _controllerObjects;
        public IController[] Controllers;
        public List<CloneSpawn> CloneSpawns;
        public List<DirectionalCloneSpawn> DirectionalCloneSpawns;
        
        private void Awake()
        {
            _walkables = transform.GetComponentsInChildren<Walkable>();
            _colorables = transform.GetComponentsInChildren<Colorable>();
            Controllers = transform.GetComponentsInChildren<IController>();
            
            _controllerObjects = new GameObject[Controllers.Length];
            var controllerCount = Controllers.Length;
            for (var i = 0; i < controllerCount; ++i)
            {
                _controllerObjects[i] = ((MonoBehaviour) Controllers[i]).gameObject;
            }
            
            CloneSpawns = new List<CloneSpawn>();
            DirectionalCloneSpawns = new List<DirectionalCloneSpawn>();
        }
        
        public void FindNeighbors()
        {
            NUM_WALKABLES = 0;
            foreach (var walkable in _walkables)
            {
                ++NUM_WALKABLES;
                walkable.CheckForNeighbors();
            }

            foreach (var colorable in _colorables)
            {
                if (colorable.CompareTag("CloneSpawn"))
                {
                    if (colorable.transform.HasComponent<DirectionalCloneSpawn>(out var spawn))
                        DirectionalCloneSpawns.Add(spawn);
                    else
                        CloneSpawns.Add(colorable.GetComponent<CloneSpawn>());
                }

                colorable.Initialize();
            }
        }
        
        public void UpdateColorables()
        {
            /*foreach (var controller in Controllers)
            {
                if (((MonoBehaviour) controller).transform.HasComponent<Colorable>(out var c))
                    c.UpdateState();
            }

            foreach (var colorable in _colorables.Where(c => !_controllerObjects.Contains(c.gameObject)))
            {
                colorable.UpdateState();
            }*/

            /*var colorableCount = _colorables.Length;
            for (var i = 0; i < colorableCount; ++i)
            {
                _colorables[i].UpdateState();
            }*/

        }
    }
}