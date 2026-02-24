using IslandCaller.App.Models;

namespace IslandCaller.App.ViewModel.Pages
{
    public class GeneralPageViewModel
    {
        private bool _isbreakdisable;
        public bool IsBreakDisable
        {
            get => _isbreakdisable;
            set { if (_isbreakdisable != value) _isbreakdisable = value; SaveSettings(); }
        }
        public void SaveSettings()
        {
            Settings.Instance.General.BreakDisable = IsBreakDisable;
        }
        public GeneralPageViewModel()
        {
            IsBreakDisable = Settings.Instance.General.BreakDisable;
        }
    }
}
