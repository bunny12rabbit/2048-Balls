using Controllers;
using Generics.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsWindow : BaseWindow
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider effectsSlider;


        [UsedImplicitly]
        public void ExitToMainMenu()
        {
            Hide();
            LevelLoader.Instance.LoadLevel(Constants.SceneIndexes.MainMenu);
        }
    }
}