using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Level;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class GlassesController : MonoBehaviour
    {
        public enum OcularState
        {
            Z,
            
            A , // red
            AB, // orange
            B , // yellow
            BC, // green
            C , // blue
            AC, // magenta
            
            Null,
        }

        private readonly OcularState[] _states =
        {
            OcularState.A,
            OcularState.AB,
            OcularState.B,
            OcularState.BC,
            OcularState.C,
            OcularState.AC
        };
    
        public static Action OnGlassesToggled;

        [ShowInInspector, ReadOnly]
        public static OcularState CurrentOcularState;
    
        private int _index;
    
        private List<PlaySoundResult> _musicStreams;
    
        private MapController _mapController;
        private MapController MapController
        {
            get
            {
                if (_mapController == null)
                    _mapController = GetComponent<LevelController>().CurrentLevelInfo.GetComponent<MapController>();
                return _mapController;
            }
        }

        public void BlankState()
        {
            CurrentOcularState = OcularState.Z;
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
        }
        
        private void Awake()
        {
            _musicStreams = new List<PlaySoundResult>();
        }

        private IEnumerator Start()
        {
            _musicStreams.Add(MasterAudio.PlaySound("World1_A"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_B"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_C"));
            
            yield return new WaitForFixedUpdate();
            _index = 0;
            CurrentOcularState = _states[(_index%6 + 6)%6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
            
            
            LevelController.OnLevelLoaded += UpdateOcularState;
        }

        private void Update()
        {
            var left = Input.GetKeyDown(KeyCode.Q);
            var right = Input.GetKeyDown(KeyCode.E);
            if (!(left && right) && !ColorWheel.Turning && !Pathfinder.Navigating)
            {
                if (left)
                {
                    _index++;
                    UpdateOcularState();
                }

                if (right)
                {
                    _index--;
                    UpdateOcularState();
                }
            }
        }

        private void UpdateOcularState()
        {
            CurrentOcularState = _states[(_index%6 + 6)%6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
        }
    }
}
