using IslandCaller.App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IslandCaller.App.ViewModel.Pages
{
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        public ProfilePageViewModel()
        {
            _ispreferprofile = Settings.Instance.Profile.IsPreferProfile;
            _profilelist = new ObservableCollection<KeyValuePair<Guid, string>>(Settings.Instance.Profile.ProfileList);
            _profilenum = Settings.Instance.Profile.ProfileNum;
            _defaultprofilename = Settings.Instance.Profile.DefaultProfile;
            _preferprofile = new ObservableCollection<KeyValuePair<Guid, string>>(Settings.Instance.Profile.ProfilePrefer);
            _preferprofilenum = _preferprofile.Count;
        }

        private bool _ispreferprofile;
        public bool IsPreferProfile
        {
            get => _ispreferprofile;
            set 
            { 
                if (_ispreferprofile != value) _ispreferprofile = value; 
                OnPropertyChanged(nameof(IsPreferProfile));
                SaveSettings(); 
            }
        }

        private ObservableCollection<KeyValuePair<Guid, string>> _profilelist;
        public ObservableCollection<KeyValuePair<Guid, string>> ProfileList
        {
            get => _profilelist;
            set { if (_profilelist != value) _profilelist = value; OnPropertyChanged(nameof(ProfileList)); }
        }

        private int _profilenum;
        public int ProfileNum
        {
            get => _profilenum;
            set { if (_profilenum != value) _profilenum = value; OnPropertyChanged(nameof(ProfileNum)); }
        }

        private int _preferprofilenum;
        public int PreferProfileNum
        {
            get => _preferprofilenum;
            set { if (_preferprofilenum != value) _preferprofilenum = value; OnPropertyChanged(nameof(PreferProfileNum)); }
        }

        private Guid _defaultprofilename;
        public Guid DefaultProfileName
        {
            get => _defaultprofilename;
            set { if (_defaultprofilename != value) _defaultprofilename = value; OnPropertyChanged(nameof(DefaultProfileName)); }
        }

        private ObservableCollection<KeyValuePair<Guid, string>> _preferprofile;
        public ObservableCollection<KeyValuePair<Guid, string>> PreferProfile
        {
            get => _preferprofile;
            set { if (_preferprofile != value) _preferprofile = value; OnPropertyChanged(nameof(PreferProfile)); }
        }

        public void SaveSettings()
        {
            if (!PreferProfile.Any()) IsPreferProfile = false;
            ProfileNum = ProfileList.Count;
            Settings.Instance.Profile.ProfileNum = ProfileNum;
            Settings.Instance.Profile.DefaultProfile = DefaultProfileName;
            Settings.Instance.Profile.IsPreferProfile = IsPreferProfile; 
            Settings.Instance.Profile.ProfileList = ProfileList.ToDictionary(kv => kv.Key, kv => kv.Value);
            Settings.Instance.Profile.ProfilePrefer = PreferProfile.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
