using IslandCaller.App.Models;

namespace IslandCaller.App.ViewModel.Pages
{
    internal class SecurityPageViewModel
    {
        private bool _islockon;
        public bool IsLockOn
        {
            get => _islockon;
            set 
            { if (_islockon != value) 
                {
                    _islockon = value;
                    SaveSettings();
                }
            }
        }

        private int _encryptionmode;
        public int EncryptionMode
        {
            get => _encryptionmode;
            set 
            { 
                if (_encryptionmode != value) 
                { 
                    _encryptionmode = value; 
                    SaveSettings(); 
                }
            }
        }

        public void SaveSettings()
        {
            if (EncryptionMode == 0)  IsLockOn = false;
            Settings.Instance.Security.EncryptionMode = EncryptionMode;
            Settings.Instance.Security.IsLockOn = IsLockOn;
        }
        public void ReloadSettings()
        {
            _islockon = Settings.Instance.Security.IsLockOn;
            _encryptionmode = Settings.Instance.Security.EncryptionMode;
        }
        public SecurityPageViewModel()
        {
            _islockon = Settings.Instance.Security.IsLockOn;
            _encryptionmode = Settings.Instance.Security.EncryptionMode;
        }
    }
}
