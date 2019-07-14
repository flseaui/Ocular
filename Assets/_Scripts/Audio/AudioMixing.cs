using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class AudioMixing
    {
        private static readonly float WorldOneAVolume = 0.4f;
        private static readonly float WorldOneBVolume = 1.0f;
        private static readonly float WorldOneCVolume = 0.5f;

        public static readonly float[] WorldOneVolumes =
        {
            WorldOneAVolume,
            WorldOneBVolume,
            WorldOneCVolume
        };
    }
}
