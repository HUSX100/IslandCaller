using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Shared;
using IslandCaller.Models;
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
            var logger = IAppHost.GetService<ILogger<Plugin>>();
            AppBase.Current.AppStarted += async (_, _) =>
            {
                try
                {
                    new Settings().Load();
                    await new Status().LoadStatus();
                }
                catch (Exception ex)
                {
                    logger.LogError($"初始化失败：{ex}");
                    return;
                }
            };
        }
    }
}