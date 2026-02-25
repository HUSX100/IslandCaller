using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using IslandCaller.Models;
using IslandCaller.Plugin2.Services;
using IslandCaller.Services.IslandCallerService;
using IslandCaller.Services.NotificationProvidersNew;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IslandCaller
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            var logger = IAppHost.TryGetService<ILogger<Plugin>>();
            services.AddNotificationProvider<IslandCallerNotificationProviderNew>();
            services.AddSingleton<IslandCallerService>();
            services.AddSingleton<ProfileService>();
            services.AddSingleton<HistoryService>();
            services.AddSingleton<CoreService>();
            AppBase.Current.AppStarted += async (_, _) =>
            {
                try
                {
                    logger = IAppHost.GetService<ILogger<Plugin>>();
                    new Settings(IAppHost.GetService<ProfileService>()).Load();
                    logger.LogDebug("设置加载完成，正在加载默认配置...");
                    IAppHost.GetService<ProfileService>().LoadSelectedProfile(Settings.Instance.Profile.DefaultProfile);
                    logger.LogDebug("默认配置加载完成，正在加载历史记录...");
                    IAppHost.GetService<HistoryService>().Load(Settings.Instance.Profile.DefaultProfile);
                    logger.LogDebug("历史记录加载完成，正在初始化核心服务...");
                    IAppHost.GetService<CoreService>().InitializeCore();
                    logger.LogDebug("核心服务初始化完成，正在启动 IslandCaller 服务...");
                    IAppHost.GetService<IslandCallerService>();
                    logger.LogInformation("IslandCaller 插件初始化完成");
                }
                catch (Exception ex)
                {
                    logger = IAppHost.GetService<ILogger<Plugin>>();
                    logger.LogError($"初始化失败：{ex}");
                    return;
                }
            };
        }
    }
}