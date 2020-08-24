using Managers;

namespace UI
{
    public class VictoryWindow : SettingsWindow
    {
        public override void OnShow()
        {
            UiManager.Instance.SetPauseButtonActive(false);
            base.OnShow();
        }
    }
}