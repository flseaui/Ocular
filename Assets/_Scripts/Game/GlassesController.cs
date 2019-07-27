using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Level;
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

        private OcularState[] states =
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
    
        private int index;
    
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
            index = 0;
            CurrentOcularState = states[Math.Abs(index) % 6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
            
            
            LevelController.OnLevelLoaded += UpdateOcularState;
        }

        private void Update()
        {
            var left = Input.GetKeyDown(KeyCode.Q);
            var right = Input.GetKeyDown(KeyCode.E);
            if (!(left && right))
            {
                if (left)
                {
                    index--;
                    UpdateOcularState();
                }

                if (right)
                {
                    index++;
                    UpdateOcularState();
                }
            }
        }
        
        public void UpdateOcularState()
        {
            CurrentOcularState = states[Math.Abs(index) % 6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
        }
    }
}
