using System.Collections.Generic;
using StaticTools;
using UnityEngine;

namespace Managers
{
    public class ParticleSystemManager : MonoBehaviour
    {
        private const float COLOR_CORRECTION_FACTOR = -0.25f;


        [SerializeField] private List<ParticleSystem> targets;


        private ParticleSystem.MainModule[] _mainModules;

        private void Awake()
        {
            if (targets == null)
            {
                Debug.LogError(
                    $"::{nameof(ParticleSystemManager)}:: {nameof(targets)} field is null. Assign references!");
                return;
            }

            InitializeMainModules();
        }

        private void InitializeMainModules()
        {
            _mainModules = new ParticleSystem.MainModule[targets.Count];

            for (var index = 0; index < targets.Count; index++)
                _mainModules[index] = targets[index].main;
        }

        public void ChangeColor(Color color)
        {
            var darkColor = StaticUtilities.ChangeColorBrightness(color, COLOR_CORRECTION_FACTOR);
            var minMaxGradient = new ParticleSystem.MinMaxGradient(darkColor, color);
            foreach (var mainModule in _mainModules)
            {
                var module = mainModule;
                module.startColor = minMaxGradient;
            }
        }
    }
}