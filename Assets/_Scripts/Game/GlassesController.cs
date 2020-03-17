using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Audio;
using DarkTonic.MasterAudio;
using Level;
using Player;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI;
using UI.Settings.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private int CurrentWorld => GetComponent<LevelController>().GetCurrentWorld();

        private int _currentWorldMusic;

        public void BlankState()
        {
            CurrentOcularState = OcularState.Z;
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
        }
        
        private void Awake()
        {   
            index = 0;
            _musicStreams = new List<PlaySoundResult>();

            SpotlightController.SpotlightEnabled += UpdateOcularState;

            LevelController.OnLevelLoaded += OnLevelLoaded;
            EntityManager.OnEntitiesSpawned += OnEntitiesSpawned;

            _currentWorldMusic = -1;
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name != "NewEditor")
                StartCoroutine(UpdateVolume());
        }

        private IEnumerator UpdateVolume()
        {
            //TODO jank audio fix
            
            yield return new WaitUntil(() => MasterAudio.SoundsReady);
            
            MasterAudio.PauseMixer();
            if (PersistentAudioSettings.MixerVolume.HasValue)
                MasterAudio.MasterVolumeLevel = PersistentAudioSettings.MixerVolume.Value;
            MasterAudio.UnpauseMixer();
        }
        
        private void OnEntitiesSpawned()
        {
            UpdateOcularState();
        }
        
        private void OnLevelLoaded()
        {
            _mapController = GetComponent<LevelController>().CurrentLevelInfo.GetComponent<MapController>();
            //UpdateOcularState();
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= OnLevelLoaded;
        }

        public void UpdateOcularState()
        {
            CurrentOcularState = _states[(index % 6 + 6) % 6];
            MapController.UpdateColorables();
            OnGlassesToggled?.Invoke();
            
            if (PlayerPrefs.GetInt("PlayFromEditor") == 0)
                UpdateMusicStreams();
        }

        public void CheckForNewWorldMusic()
        {
           if(_musicStreams.IsNullOrEmpty() || _currentWorldMusic != CurrentWorld) SetMusicStreams();
        }

        private void SetMusicStreams()
        {
            var currentWorld = CurrentWorld;
            
            // TODO REMOVE
            if (CurrentWorld > 1)
                currentWorld = 1;

            if (_currentWorldMusic == currentWorld) return;
            
            if (_musicStreams.Count != 0)
            {
                _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                _musicStreams[3].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                _musicStreams.Clear();
            }
            _currentWorldMusic = CurrentWorld;
            _musicStreams.Add(MasterAudio.PlaySound($"World{currentWorld + 1}_A", AudioMixing.WorldVolumes[currentWorld][0]));
            _musicStreams.Add(MasterAudio.PlaySound($"World{currentWorld + 1}_B", AudioMixing.WorldVolumes[currentWorld][1]));
            _musicStreams.Add(MasterAudio.PlaySound($"World{currentWorld + 1}_C", AudioMixing.WorldVolumes[currentWorld][2]));
            _musicStreams.Add(MasterAudio.PlaySound($"World{currentWorld + 1}_Main", AudioMixing.WorldVolumes[currentWorld][3]));
            
            _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[currentWorld][0], AudioMixing.FadeInTime * 2);
            _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[currentWorld][1], AudioMixing.FadeInTime * 2);
            _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[currentWorld][2], AudioMixing.FadeInTime * 2);
            _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[currentWorld][3], AudioMixing.FadeInTime * 2);
        }

        private void UpdateMusicStreams()
        {       
            //TODO REMOVE
            if (CurrentWorld > 1)
                return;
            
            switch (CurrentOcularState)
            {
                case OcularState.Z:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.A:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.AB:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.B:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.BC:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][1], AudioMixing.FadeInTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.C:
                    _musicStreams[0].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
                    break;
                case OcularState.AC:
                    _musicStreams[0].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][0], AudioMixing.FadeInTime);
                    _musicStreams[1].ActingVariation.FadeToVolume(0, AudioMixing.FadeOutTime);
                    _musicStreams[2].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][2], AudioMixing.FadeInTime);
                    _musicStreams[3].ActingVariation.FadeToVolume(AudioMixing.WorldVolumes[CurrentWorld][3], AudioMixing.FadeInTime);
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
            
            MasterAudio.PauseMixer();
            if (PersistentAudioSettings.MixerVolume.HasValue)
                MasterAudio.MasterVolumeLevel = PersistentAudioSettings.MixerVolume.Value;
            MasterAudio.UnpauseMixer();
        }
    }
}
