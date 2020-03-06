using System.Collections;
using System.Linq;
using Boo.Lang;
using Level.Objects;
using Misc;
using UnityEngine;

namespace Level {
    public class MapController : MonoBehaviour
    {
        public static int NUM_WALKABLES;

        private Walkable[] _walkables;
        private Colorable[] _colorables;
        public IController[] Controllers;
        
        private void Awake()
        {
            _walkables = transform.GetComponentsInChildren<Walkable>();
            _colorables = transform.GetComponentsInChildren<Colorable>();
            Controllers = transform.GetComponentsInChildren<IController>();
        }
        
        public void FindNeighbors()
        {

            NUM_WALKABLES = 0;
            foreach (var walkable in _walkables)
            {
                ++NUM_WALKABLES;
                walkable.CheckForNeighbors();
            }

            foreach (var colorable in _colorables) colorable.Initialize();
        }
        
        public void UpdateColorables()
        {
            var controllers = new List<GameObject>();
            foreach (var controller in Controllers)
            {
                if (((MonoBehaviour) controller).transform.HasComponent<Colorable>(out var c))
                    c.UpdateState();
                controllers.Add(((MonoBehaviour) controller).gameObject);
            }

            foreach (var colorable in _colorables.Where(c => !controllers.Contains(c.gameObject)))
            {
                colorable.UpdateState();
            }
        }
    }
}