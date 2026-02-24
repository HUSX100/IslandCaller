using IslandCaller.App.Models;

namespace IslandCaller.App.ViewModel.Pages
{
    internal class HoverPageViewModel
    {
        private bool _ishoveron;
        public bool IsHoverOn
        {
            get => _ishoveron;
            set 
            { if (_ishoveron != value) 
                {
                    _ishoveron = value;
                    if (IsHoverOn) Status.Instance.fluenthover.Show();
                    else Status.Instance.fluenthover.Hide();
                    SaveSettings();
                }
            }
        }
        public void SaveSettings()
        {
            Settings.Instance.Hover.IsEnable = IsHoverOn;
        }
        public HoverPageViewModel()
        {
            IsHoverOn = Settings.Instance.Hover.IsEnable;
        }
    }
}
