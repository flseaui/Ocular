using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
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

        private MapController MapController => _mapController;

        public void BlankState()
        {
            CurrentOcularState = OcularState.Z;
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
        }
        
        private void Awake()
        {
            _musicStreams = new List<PlaySoundResult>();
            LevelController.OnLevelLoaded += () =>
            {
                _mapController = GetComponent<LevelController>().CurrentLevelInfo.GetComponent<MapController>();
            };
        }

        private IEnumerator Start()
        {
            
            _musicStreams.Add(MasterAudio.PlaySound("World1_A"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_B"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_C"));
            
            yield return new WaitForFixedUpdate();
            _index = 0;

            LevelController.OnLevelLoaded += UpdateOcularState;
        }

        private void Update()
        {
            var left = Input.GetKeyDown(KeyCode.Q);
            var right = Input.GetKeyDown(KeyCode.E);
            if (!(left && right) && !ColorWheel.Turning && !Pathfinder.Navigating && !Player.Player.Falling)
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
            UpdateMusicStreams();
        }

        private void UpdateMusicStreams()
        {
            switch (CurrentOcularState)
            {
                case OcularState.Z:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], 1);
                    break;
                case OcularState.A:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, 1);
                    break;
                case OcularState.AB:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, 1);
                    break;
                case OcularState.B:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, 1);
                    break;
                case OcularState.BC:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], 1);
                    break;
                case OcularState.C:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], 1);
                    break;
                case OcularState.AC:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], 1);
                    break;
                case OcularState.Null:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, 1);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
