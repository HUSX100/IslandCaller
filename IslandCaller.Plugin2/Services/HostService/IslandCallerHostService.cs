using IslandCaller.Services.NotificationProvidersNew;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared.Enums;
using IslandCaller.Models;
using Microsoft.Extensions.Hosting;
using IslandCaller.Plugin2.Services;

namespace IslandCaller.Services.IslandCallerService
{
    public class IslandCallerService
    {

        public IslandCallerService(Plugin plugin, 
                                    IUriNavigationService uriNavigationService, 
                                    ILessonsService lessonsService,
                                    HistoryService historyService,
                                    CoreService coreService)
        {
            lessonsService.CurrentTimeStateChanged += (s, e) =>
            {
                historyService.ClearThisLessonHistory();
            };
            uriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Run",
                args =>
                {
                    new IslandCallerNotificationProviderNew(lessonsService,coreService).RandomCall(1);
                }
            );
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
                    new IslandCallerNotificationProviderNew(lessonsService,coreService).RandomCall(1);
                }
            );
        }
    }
}
