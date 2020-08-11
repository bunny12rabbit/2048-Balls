using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Sound
    {
        public AudioFxTypes type;

        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1;
        [Range(.1f, 3f)] public float pitch = 1;

        public bool loop;
    }
}