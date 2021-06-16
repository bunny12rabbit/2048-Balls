using System;
using Generics;
using Pixelplacement;
using UnityEngine;

namespace Managers
{
    public class SceneTransitionManager : SingletonBehaviourPersistentGeneric<SceneTransitionManager>
    {
        public Action onLoadingAnimationDone;

        [SerializeField] private Transform loadingScreenTransform;

        [SerializeField] private float loadingAnimationDuration = 1f;

        private bool _isLoadingAnimationPlaying;

        private Vector3 _loadingScreenOriginPosition;
        private Vector3 _loadingScreenFinishPosition;

        public void PlayLoadingScreenAnimation(bool isShow)
        {
            if (_isLoadingAnimationPlaying)
                return;

            _isLoadingAnimationPlaying = true;
            var endPosition = isShow ? Vector3.zero : _loadingScreenFinishPosition;

            Tween.LocalPosition(loadingScreenTransform, endPosition, loadingAnimationDuration * 0.5f, 0, null,
                Tween.LoopType.None, null, OnLoadingAnimationDone);
        }

        private void Start()
        {
            _loadingScreenOriginPosition = loadingScreenTransform.localPosition;
            _loadingScreenFinishPosition = new Vector3(-_loadingScreenOriginPosition.x, 0, 0);
        }

        private void OnLoadingAnimationDone()
        {
            _isLoadingAnimationPlaying = false;
            onLoadingAnimationDone?.Invoke();
        }
    }
}