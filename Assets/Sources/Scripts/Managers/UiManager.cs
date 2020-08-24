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
        
        [Space]
        [SerializeField] private UIButton pauseButton;

        [SerializeField] private GameObject logo;


        public GameObject Canvas => canvas;

        public Transform ActiveWindowsContainer => activeWindowsContainer;

        public Transform InactiveWindowsContainer => inactiveWindowsContainer;

        public void ShowGameUi() => SetPauseButtonActive(true);

        public void ShowMainMenuUi()
        {
            WindowManager.Instance.ShowWindow(typeof(MainMenuWindow));
            SetPauseButtonActive(false);
            logo.SetActive(false);
        }

        [UsedImplicitly]
        public void ShowPauseWindow() => WindowManager.Instance.ShowWindow(typeof(SettingsWindow));


        private void Start() => SetPauseButtonActive(false);

        private void SetPauseButtonActive(bool isActive) => pauseButton.gameObject.SetActive(isActive);
    }
}