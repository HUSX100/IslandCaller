using ClassIsland.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.Models
{
    public class Status
    {
        public static Status Instance { get; } = new Status();
        public static Profile profile = new();
        public static History history = new(); 
        public TimeState lessonstatu;
        public async Task LoadStatus()
        {
            await profile.Load(Settings.Instance.Profile.DefaultProfile);
            await history.LoadAsync(Settings.Instance.Profile.DefaultProfile);
        }
    }
}
