using Controllers;
using Generics.UI;
using JetBrains.Annotations;

namespace UI
{
    public class SettingsWindow : BaseWindow
    {
        [UsedImplicitly]
        public void ExitToMainMenu()
        {
            Hide();
            LevelLoader.Instance.LoadLevel(Constants.SceneIndexes.MainMenu);
        }
    }
}