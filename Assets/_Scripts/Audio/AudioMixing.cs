using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class AudioMixing
    {
        private static readonly float WorldOneAVolume = 0.4f;
        private static readonly float WorldOneBVolume = .4f;
        private static readonly float WorldOneCVolume = 0.5f;
        private static readonly float WorldOneMainVolume = .7f;

        public static readonly float FadeInTime = 1.5f;
        public static readonly float FadeOutTime = 3f;

        public static readonly float[] WorldOneVolumes =
        {
            WorldOneAVolume,
            WorldOneBVolume,
            WorldOneCVolume,
            WorldOneMainVolume
        };
    }
}
