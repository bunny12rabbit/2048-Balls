using JetBrains.Annotations;
using Managers;
using StaticTools;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private string volumeParameter;

        [SerializeField] private Slider slider;


        private float _volume;


        [UsedImplicitly]
        public void SetLevel(float sliderValue)
        {
            _volume = StaticUtilities.GetLogValueForMixer(sliderValue);

            AudioManager.Instance.SetVolume(volumeParameter, _volume);
        }

        private void UpdateSlider() => slider.value = PlayerPrefs.GetFloat(volumeParameter, 0.5f);

        private void OnEnable()
        {
            UpdateSlider();
        }

        private void OnDisable() => SaveVolume();

        private void SaveVolume() => PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}