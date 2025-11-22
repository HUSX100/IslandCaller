using IslandCaller.App.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace IslandCaller.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.WriteLog("App.xaml.cs", "Debug", "IslandCaller Plugin Initializing");
            new Settings().Load();
            Settings.Instance.Profile.ProfileList.TryGetValue(Settings.Instance.Profile.DefaultProfile, out string value);
            Core.RandomImport(value);
            Status.Instance.fluenthover.Show();
        }
    }

}
