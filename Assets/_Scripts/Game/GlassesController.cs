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
    
        public int index;
    
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
            _musicStreams.Add(MasterAudio.PlaySound("World1_Main"));
            
            yield return new WaitForFixedUpdate();
            index = 0;

            LevelController.OnLevelLoaded += UpdateOcularState;
        }

        public void UpdateOcularState()
        {
            CurrentOcularState = _states[(index%6 + 6)%6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
            UpdateMusicStreams();
        }

        private void UpdateMusicStreams()
        {
            switch (CurrentOcularState)
            {
                case OcularState.Z:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.A:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.AB:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.B:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.BC:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.C:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.AC:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldOneVolumes[3], AudioMixing.FadeInTime);
                    break;
                case OcularState.Null:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
