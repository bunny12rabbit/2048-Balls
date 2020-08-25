using System;
using System.Collections.Generic;
using Controllers;
using Generics;
using Generics.UI;
using JetBrains.Annotations;
using UI;
using UnityEngine;

namespace Managers
{
    public class UiManager : SingletonBehaviourPersistentGeneric<UiManager>
    {
        [SerializeField] private GameObject canvas;

        [SerializeField] private Transform activeWindowsContainer;
        [SerializeField] private Transform inactiveWindowsContainer;

        [Space] [SerializeField] private UIButton pauseButton;

        [SerializeField] private GameObject logo;


        private Dictionary<Constants.SceneIndexes, Action> _onSceneLoadedActions;

        public GameObject Canvas => canvas;

        public Transform ActiveWindowsContainer => activeWindowsContainer;

        public Transform InactiveWindowsContainer => inactiveWindowsContainer;


        [UsedImplicitly]
        public void ShowPauseWindow() => WindowManager.Instance.ShowWindow(typeof(SettingsWindow));

        public void SetPauseButtonActive(bool isActive) => pauseButton.gameObject.SetActive(isActive);


        protected override void Awake()
        {
            base.Awake();

            _onSceneLoadedActions = new Dictionary<Constants.SceneIndexes, Action>()
            {
                {Constants.SceneIndexes.MainMenu, ShowMainMenuUi}, {Constants.SceneIndexes.Game, ShowGameUi}
            };
        }

        private void OnEnable()
        {
            LevelLoader.onSceneLoaded += OnSceneLoaded;
            SetPauseButtonActive(false);
        }

        private void OnDisable() => LevelLoader.onSceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Constants.SceneIndexes scene) => _onSceneLoadedActions[scene]?.Invoke();

        private void ShowGameUi() => SetPauseButtonActive(true);

        private void ShowMainMenuUi()
        {
            WindowManager.Instance.ShowWindow(typeof(MainMenuWindow));
            SetPauseButtonActive(false);
            logo.SetActive(false);
        }
    }
}