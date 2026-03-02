using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using IslandCaller.Helpers;
using IslandCaller.Models;
using IslandCaller.Services;
using IslandCaller.Services.IslandCallerService;
using IslandCaller.Services.NotificationProvidersNew;
using IslandCaller.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace IslandCaller
{
    [PluginEntrance]
    public class Plugin : PluginBase, INotifyPropertyChanged
    {
        private bool _pluginStatus;
        public bool PluginStatus
        {
            get => _pluginStatus;
            set
            {
                if (_pluginStatus != value)
                {
                    _pluginStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PluginStatus)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            PluginStatus = true;
            var logger = IAppHost.TryGetService<ILogger<Plugin>>();
            services.AddNotificationProvider<IslandCallerNotificationProviderNew>();
            services.AddSingleton<IslandCallerService>();
            services.AddSingleton<ProfileService>();
            services.AddSingleton<HistoryService>();
            services.AddSingleton<CoreService>();
            services.AddSingleton<WindowDragHelper>();
            services.AddSettingsPage<SettingPage>();
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
                    new HoverFluent().Show();
                }
                catch (Exception ex)
                {
                    PluginStatus=false;
                    logger = IAppHost.GetService<ILogger<Plugin>>();
                    logger.LogCritical($"初始化失败：{ex}");
                    throw;
                }

            };
        }
    }
}