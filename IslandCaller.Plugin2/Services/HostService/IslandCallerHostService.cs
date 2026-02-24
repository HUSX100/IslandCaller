using IslandCaller.Services.NotificationProvidersNew;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared.Enums;
using IslandCaller.Models;
using Microsoft.Extensions.Hosting;
using Status = IslandCaller.Models.Status;

namespace IslandCaller.Services.IslandCallerHostService
{
    public class IslandCallerHostService : IHostedService
    {
        private ILessonsService LessonsService { get; }
        public IUriNavigationService UriNavigationService { get; }

        public IslandCallerHostService(Plugin plugin, IUriNavigationService uriNavigationService, ILessonsService lessonsService)
        {
            LessonsService = lessonsService;
            UriNavigationService = uriNavigationService;
            lessonsService.CurrentTimeStateChanged += (s, e) =>
            {
                Status.Instance.lessonstatu = lessonsService.CurrentState;
                new History().ClearTop20();
            };
            UriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Run",
                args =>
                {
                    new IslandCallerNotificationProviderNew().RandomCall(1);
                }
            );
            UriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Simple",
                args =>
                {
                    new IslandCallerNotificationProviderNew().RandomCall(1);
                }
            );
            UriNavigationService.HandlePluginsNavigation(
                "IslandCaller/Advanced/GUI",
                args =>
                {
                    new IslandCallerNotificationProviderNew().RandomCall(1);
                }
            );
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
