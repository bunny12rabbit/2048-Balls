using Controllers;
using Generics.UI;
using JetBrains.Annotations;
using Managers;

namespace UI
{
    public class MainMenuWindow : BaseWindow
    {
        [UsedImplicitly]
        public void LoadGameScene()
        {
            SceneTransitionManager.Instance.onLoadingAnimationDone += HideThisWindow;
            LevelLoader.Instance.LoadLevel(Constants.SceneIndexes.Game);
        }

        private void HideThisWindow()
        {
            WindowManager.Instance.HideWindow(this);
            SceneTransitionManager.Instance.onLoadingAnimationDone -= HideThisWindow;
        }

        [UsedImplicitly]
        public void ShowSettingsWindow() => WindowManager.Instance.ShowWindow(typeof(SettingsWindow));
    }
}