using Audio;
using DarkTonic.MasterAudio;
using UnityEngine;

namespace UI.Settings.Audio
{
    public class VolumeManager : MonoBehaviour
    {
        public void OnMasterVolumeChanged(float value)
        {
            AudioMixing.MasterVolume = value / 100 / 3;
            Debug.Log(value);
            PersistentAudioSettings.MusicVolume = value / 100;
        }
        public void OnMusicVolumeChanged(float value)
        {
            PersistentAudioSettings.MusicVolume = value / 100;
            AudioMixing.MasterMusicVolume = value / 100 / 3;
        }
        public void OnSFXVolumeChanged(float value)
        {
            AudioMixing.MasterEffectVolume = value / 100 / 3;
        }
    }
}
