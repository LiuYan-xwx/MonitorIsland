using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Interfaces;
using MonitorIsland.Services;

namespace MonitorIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddComponent<MonitorComponent, MonitorComponentSettingsControl>();
        services.AddSingleton<IMonitorService, MonitorService>();
    }
}
