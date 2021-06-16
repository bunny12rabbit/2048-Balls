using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum AudioFxTypes
    {
        Click,
        Collapse,
        Win,
        Music
    }

    [CreateAssetMenu(fileName = "AudioDataCollection", menuName = "AudioData", order = 0)]
    public class AudioData : ScriptableObject
    {
        [SerializeField] private Sound[] musicClips;
        [Space] [SerializeField] private Sound[] fxClips;

        private Dictionary<AudioFxTypes, Sound> _fxClipsTable;


        private int _lastSong = -1;


        public Sound GetRandomMusicSound()
        {
            int randomIndex = _lastSong;

            while (randomIndex == _lastSong)
                randomIndex = Random.Range(0, musicClips.Length);

            _lastSong = randomIndex;

            return musicClips[randomIndex];
        }

        public Sound GetFxSound(AudioFxTypes type)
        {
            if (_fxClipsTable == null)
                InitializeFxClipsTable();

            return _fxClipsTable[type];
        }

        private void InitializeFxClipsTable()
        {
            _fxClipsTable = new Dictionary<AudioFxTypes, Sound>(fxClips.Length);

            foreach (var fxClip in fxClips)
                _fxClipsTable.Add(fxClip.type, fxClip);
        }
    }
}