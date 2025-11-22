using static System.Guid;
using System.ComponentModel;

namespace IslandCaller.App.Models
{
    public class SettingsModel
    {
        public GeneralSetting General { get; set; } = new GeneralSetting();
        public ProfileSetting Profile { get; set; } = new ProfileSetting();
        public HoverSetting Hover { get; set; } = new HoverSetting();
        public SecuritySetting Security { get; set; } = new SecuritySetting();
    }

    public class GeneralSetting : INotifyPropertyChanged
    {
        public GeneralSetting()
        {
            _version = "1.0.7.0";
            _breakdisable = false;
        }

        private string _version;
        public string Version
        {
            get => _version;
        }

        private bool _breakdisable;
        public bool BreakDisable
        {
            get => _breakdisable;
            set { if (_breakdisable != value) { _breakdisable = value; OnPropertyChanged(nameof(BreakDisable)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ProfileSetting : INotifyPropertyChanged
    {
        public ProfileSetting()
        {
            _profilenum = 1;
            _defaultprofile = NewGuid();
            _profilelist.Add(_defaultprofile, "Default");
            _ispreferprofile = false;
        }

        private int _profilenum;
        public int ProfileNum
        {
            get => _profilenum;
            set { if (_profilenum != value) { _profilenum = value; OnPropertyChanged(nameof(ProfileNum)); } }
        }

        private Guid _defaultprofile;
        public Guid DefaultProfile
        {
            get => _defaultprofile;
            set { if (_defaultprofile != value) { _defaultprofile = value; OnPropertyChanged(nameof(DefaultProfile)); } }
        }

        private Dictionary<Guid, string> _profilelist = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> ProfileList
        {
            get => _profilelist;
            set { if (_profilelist != value) { _profilelist = value; OnPropertyChanged(nameof(ProfileList)); } }
        }
        private Dictionary<Guid, string> _profileprefer = new Dictionary<Guid, string>();

        private bool _ispreferprofile;
        public bool IsPreferProfile
        {
            get => _ispreferprofile;
            set { if (_ispreferprofile != value) { _ispreferprofile = value; OnPropertyChanged(nameof(IsPreferProfile)); } }
        }
        public Dictionary<Guid, string> ProfilePrefer
        {
            get => _profileprefer;
            set { if (_profileprefer != value) { _profileprefer = value; OnPropertyChanged(nameof(ProfilePrefer)); } }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class HoverSetting : INotifyPropertyChanged
    {
        public HoverSetting()
        {
            _isEnable = true;
        }

        private bool _isEnable;
        public bool IsEnable
        {
            get => _isEnable;
            set { if (_isEnable != value) { _isEnable = value; OnPropertyChanged(nameof(IsEnable)); } }
        }

        public PositionSetting Position { get; set; } = new PositionSetting();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PositionSetting : INotifyPropertyChanged
    {
        public PositionSetting()
        {
            _x = 200.0;
            _y = 200.0;
        }

        private double _x;
        public double X
        {
            get => _x;
            set { if (_x != value) { _x = value; OnPropertyChanged(nameof(X)); } }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set { if (_y != value) { _y = value; OnPropertyChanged(nameof(Y)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SecuritySetting : INotifyPropertyChanged
    {
        public SecuritySetting()
        {
            _encryptionMode = 0; // Default to no encryption
            _islockon = false;
        }

        private int _encryptionMode;
        public int EncryptionMode
        {
            get => _encryptionMode;
            set { if (_encryptionMode != value) { _encryptionMode = value; OnPropertyChanged(nameof(EncryptionMode)); } }
        }

        private bool _islockon;
        public bool IsLockOn
        {
            get => _islockon;
            set { if (_islockon != value) { _islockon = value; OnPropertyChanged(nameof(IsLockOn)); } }
        }

        public SecretKeySetting SecretKey { get; set; } = new SecretKeySetting();
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SecretKeySetting : INotifyPropertyChanged
    {
        public SecretKeySetting()
        {
            _aesKey = string.Empty;
            _totpKey = string.Empty;
            _passkey = string.Empty;
            _password = string.Empty;
            _usbKey = string.Empty;
        }

        private string _aesKey;
        public string AESKey
        {
            get => _aesKey;
            set { if (_aesKey != value) { _aesKey = value; OnPropertyChanged(nameof(AESKey)); } }
        }

        private string _totpKey;
        public string TOTPKey
        {
            get => _totpKey;
            set { if (_totpKey != value) { _totpKey = value; OnPropertyChanged(nameof(TOTPKey)); } }
        }

        private string _passkey;
        public string Passkey
        {
            get => _passkey;
            set { if (_passkey != value) { _passkey = value; OnPropertyChanged(nameof(Passkey)); } }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { if (_password != value) { _password = value; OnPropertyChanged(nameof(Password)); } }
        }

        private string _usbKey;
        public string USBKey
        {
            get => _usbKey;
            set { if (_usbKey != value) { _usbKey = value; OnPropertyChanged(nameof(USBKey)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}