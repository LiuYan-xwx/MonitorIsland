using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Controls.MonitorProviderSettingsControls;
using MonitorIsland.Extensions;
using MonitorIsland.Interfaces;
using MonitorIsland.Providers;
using MonitorIsland.Services;

namespace MonitorIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddComponent<MonitorComponent, MonitorComponentSettingsControl>();
        services.AddSingleton<IMonitorService, MonitorService>();

        // ×¢²á¼à¿ØÌá¹©·½
        services.AddMonitorProvider<MemoryUsageProvider>();
        services.AddMonitorProvider<CpuUsageProvider>();
        services.AddMonitorProvider<MemoryUsageRateProvider>();
        services.AddMonitorProvider<DiskSpaceProvider, DiskSpaceSettingsControl>();
        services.AddMonitorProvider<ClassIslandMemoryUsageProvider>();
    }
}
