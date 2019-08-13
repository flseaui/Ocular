using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class AudioMixing
    {
        public static float MasterVolume = 1;
        public static float MasterMusicVolume = 1;
        public static float MasterEffectVolume = 1;
    
        private static readonly float WorldOneAVolume = 0.4f;
        private static readonly float WorldOneBVolume = 0.4f;
        private static readonly float WorldOneCVolume = 0.5f;
        private static readonly float WorldOneMainVolume = 0.7f;
        
        private static readonly float WorldTwoAVolume = 0.5f;
        private static readonly float WorldTwoBVolume = 0.5f;
        private static readonly float WorldTwoCVolume = 0.5f;
        private static readonly float WorldTwoMainVolume = 0.5f;

        public static readonly float FadeInTime = 1.5f;
        public static readonly float FadeOutTime = 3f;

        public static readonly float[][] WorldVolumes =
        {
            WorldOneVolumes,
            WorldTwoVolumes
        };

        private static readonly float[] WorldOneVolumes =
        {
            WorldOneAVolume * MasterVolume * MasterMusicVolume,
            WorldOneBVolume * MasterVolume * MasterMusicVolume,
            WorldOneCVolume * MasterVolume * MasterMusicVolume,
            WorldOneMainVolume * MasterVolume * MasterMusicVolume
        };

        private static readonly float[] WorldTwoVolumes =
        {
            WorldTwoAVolume * MasterVolume * MasterMusicVolume,
            WorldTwoBVolume * MasterVolume * MasterMusicVolume,
            WorldTwoCVolume * MasterVolume * MasterMusicVolume,
            WorldTwoMainVolume * MasterVolume * MasterMusicVolume
        };
    }
}
