using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Interfaces;
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

        var backendLoadException = PlatformBackendLoader.Initialize(services);
        bool disableSensorMonitor = false;
        services.TryAddSingleton<IHardwareMonitorService>(serviceProvider =>
        {
            serviceProvider.GetService<ILogger<Plugin>>()?.LogError(backendLoadException, "加载平台后端失败，将禁用传感器监控");
            disableSensorMonitor = true;
            return new UnsupportedHardwareMonitorService();
        });

        // 注册监控提供方
        services.AddMonitorProvider<MemoryUsageProvider>();
        services.AddMonitorProvider<CpuUsageProvider>();
        services.AddMonitorProvider<MemoryUsageRateProvider>();
        services.AddMonitorProvider<DiskSpaceProvider, DiskSpaceSettingsControl>();
        services.AddMonitorProvider<ClassIslandMemoryUsageProvider>();
        if (OperatingSystem.IsWindows() && !disableSensorMonitor)
        {
            services.AddMonitorProvider<SensorMonitorProvider, SensorSettingsControl>();
        }
    }
}
