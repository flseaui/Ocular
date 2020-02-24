﻿using System;
using System.Collections;
using Audio;
using DarkTonic.MasterAudio;
using UnityEngine;

namespace UI.Settings.Audio
{
    public class VolumeManager : MonoBehaviour
    {
        public static Action VolumeUpdate;

        private void Start()
        {
            // TODO
            // hardcoded default value
            AudioMixing.MasterMusicVolume = 50.1234f / 100 / 3;
            AudioMixing.MasterVolume = 99.443f / 100 / 3;

            UpdateAudioSettings();
        }

        public void OnMasterVolumeChanged(float value)
        {
            AudioMixing.MasterVolume = value / 100 / 3;
            UpdateAudioSettings();
        }
        public void OnMusicVolumeChanged(float value)
        {
            AudioMixing.MasterMusicVolume = value / 100 / 3;
            UpdateAudioSettings();
        }

        public static void UpdateAudioSettings()
        {
            PersistentAudioSettings.MusicVolume = ((AudioMixing.MasterMusicVolume * AudioMixing.MasterVolume) / 2) * 2;
        }
        
        public void OnSFXVolumeChanged(float value)
        {
            AudioMixing.MasterEffectVolume = value / 100 / 3;
        }
    }
}
