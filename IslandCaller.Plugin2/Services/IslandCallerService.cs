using IslandCaller.Services.NotificationProvidersNew;
using ClassIsland.Core.Abstractions.Services;
using IslandCaller.Views;
using ClassIsland.Shared.Enums;
using IslandCaller.Models;

namespace IslandCaller.Services.IslandCallerService
{
    public class IslandCallerService
    {
        private ILessonsService LessonsService { get; }
        private CoreService CoreService {  get; }
        private Plugin Plugin { get; }
        public IslandCallerService(Plugin plugin, 
                                    IUriNavigationService uriNavigationService, 
                                    ILessonsService lessonsService,
                                    HistoryService historyService,
                                    CoreService coreService
            )
        {
            
            LessonsService = lessonsService;
            CoreService = coreService;
            Plugin = plugin;
            lessonsService.CurrentTimeStateChanged += (s, e) =>
            {
                historyService.ClearThisLessonHistory();
                if (Settings.Instance.General.BreakDisable & lessonsService.CurrentState == TimeState.Breaking) plugin.PluginStatus = false;
                else plugin.PluginStatus = true;
            };
            uriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Simple",
                args =>
                {
                    new IslandCallerNotificationProviderNew(lessonsService,coreService).RandomCall(1);
                }
            );
            uriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Advanced/GUI",
                args =>
                {
                    new PersonalCall().Show();
                }
            );
        }

        public async void ShowRandomStudent(int stunum)
        {
            if(Plugin.PluginStatus == false) return;
            Plugin.PluginStatus = false;
            new IslandCallerNotificationProviderNew(LessonsService, CoreService).RandomCall(stunum);
            await Task.Delay(stunum * 2000 + 1000);
            Plugin.PluginStatus = true;
        }
    }
}
