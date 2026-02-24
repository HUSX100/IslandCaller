using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IslandCaller
{
    [PluginEntrance]
    public class Plugin : PluginBase
    {
        public override void Initialize(HostBuilderContext context, IServiceCollection services)
        {
            AppBase.Current.AppStarted += async (_, _) =>
                await CommonTaskDialogs.ShowDialog("Hello world!", "Hello from IslandCaller.Plugin2!");
        }
    }
}