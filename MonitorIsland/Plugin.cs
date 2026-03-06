using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Controls.MonitorProviderSettingsControls;
using MonitorIsland.Extentions;
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

        // 注册监控提供方
        services.AddMonitorProvider<MemoryUsageProvider>();
        services.AddMonitorProvider<CpuUsageProvider>();
        services.AddMonitorProvider<MemoryUsageRateProvider>();
        services.AddMonitorProvider<DiskSpaceProvider, DiskSpaceSettingsControl>();
        services.AddMonitorProvider<ClassIslandMemoryUsageProvider>();
        services.AddMonitorProvider<NetworkTrafficProvider, NetworkTrafficSettingsControl>();
    }
}
