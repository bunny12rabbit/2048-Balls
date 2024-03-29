﻿using System.Collections;
using Data;
using Generics;
using StaticTools;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : SingletonBehaviourPersistentGeneric<AudioManager>
    {
        private enum FadeType
        {
            FadeIn,
            FadeOut
        }

        [SerializeField] private AudioData audioData;

        [SerializeField] private AudioMixer mixer;
        
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource fxAudioSource;

        [SerializeField] private string musicVolumeParameter;
        [SerializeField] private string fxVolumeParameter;

        [Space] [SerializeField] private float fadeDuration = 5f;


        public void PlayFxSound(AudioFxTypes fxType)
        {
            var sound = audioData.GetFxSound(fxType);
            fxAudioSource.clip = sound.clip;
            fxAudioSource.loop = sound.loop;
            fxAudioSource.volume = sound.volume;
            fxAudioSource.pitch = sound.pitch;
            fxAudioSource.Play();
        }

        public void SetVolume(string volumeParameter, float value) => mixer.SetFloat(volumeParameter, value);

        protected override void Awake()
        {
            base.Awake();

            TrySetReferences();
        }

        private void TrySetReferences()
        {
            if (musicAudioSource == null)
                musicAudioSource = gameObject.GetOrAddComponent<AudioSource>();

            if (fxAudioSource == null)
                fxAudioSource = gameObject.GetOrAddComponent<AudioSource>();
        }

        private void Start()
        {
            UpdateVolume();
            StartMusic();
        }

        private void UpdateVolume()
        {
            float musicSliderValue = GetDataFromPrefs(musicVolumeParameter);
            float fxSliderValue = GetDataFromPrefs(fxVolumeParameter);
            
            float musicVolume = StaticUtilities.GetLogValueForMixer(musicSliderValue);
            float fxVolume = StaticUtilities.GetLogValueForMixer(fxSliderValue);

            mixer.SetFloat(musicVolumeParameter, musicVolume);
            mixer.SetFloat(fxVolumeParameter, fxVolume);
        }

        private float GetDataFromPrefs(string volumeParameter)
        {
            float defaultVolume = mixer.GetFloat(volumeParameter, out defaultVolume) ? defaultVolume : 0.5f;
            return  PlayerPrefs.GetFloat(volumeParameter, defaultVolume);
        }

        private void StartMusic()
        {
            PlayNextSong();

            StartCoroutine(ControlFade());
        }

        private void PlayNextSong()
        {
            var sound = audioData.GetRandomMusicSound();
            musicAudioSource.clip = sound.clip;
            musicAudioSource.loop = sound.loop;
            musicAudioSource.volume = sound.volume;
            musicAudioSource.pitch = sound.pitch;
            musicAudioSource.Play();
            
            StartCoroutine(Fade(FadeType.FadeIn));
        }

        private IEnumerator ControlFade()
        {
            while (true)
            {
                if (musicAudioSource.isPlaying)
                    if (musicAudioSource.clip.length - musicAudioSource.time <= fadeDuration)
                        yield return StartCoroutine(Fade(FadeType.FadeOut));

                yield return null;
            }
        }

        private IEnumerator Fade(FadeType fadeType)
        {
            if (fadeDuration > 0f)
            {
                var lerpValue = 0f;
                var clip = musicAudioSource.clip;
                float startTime = clip.length - fadeDuration;

                float targetVolume = fadeType == FadeType.FadeIn ? musicAudioSource.volume : 0;
                float inverseLerpStart = fadeType == FadeType.FadeIn ? 0 : startTime;
                float inverseLerpFinish = fadeType == FadeType.FadeIn ? fadeDuration : clip.length;
                float lerpStart = fadeType == FadeType.FadeIn ? 0 : targetVolume;
                float lerpFinish = fadeType == FadeType.FadeIn ? targetVolume : 0;


                while (lerpValue < 1f && musicAudioSource.isPlaying)
                {
                    lerpValue = Mathf.InverseLerp(inverseLerpStart, inverseLerpFinish, musicAudioSource.time);
                    musicAudioSource.volume = Mathf.Lerp(lerpStart, lerpFinish, lerpValue);
                    yield return null;
                }

                musicAudioSource.volume = targetVolume;

                if (fadeType == FadeType.FadeOut)
                    PlayNextSong();
            }
        }
    }
}