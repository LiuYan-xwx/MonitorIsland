using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Interfaces;
using MonitorIsland.Services;
using MonitorIsland.Services.Providers;

namespace MonitorIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // 注册组件
        services.AddComponent<MonitorComponent, MonitorComponentSettingsControl>();
        
        // 注册所有监控提供器
        services.AddSingleton<IMonitorProvider, MemoryProvider>();
        services.AddSingleton<IMonitorProvider, CpuUsageProvider>();
        services.AddSingleton<IMonitorProvider, CpuTemperatureProvider>();
        services.AddSingleton<IMonitorProvider, DiskSpaceProvider>();
        
        // 注册提供器工厂
        services.AddSingleton<MonitorProviderFactory>();
        
        // 注册监控服务
        services.AddSingleton<IMonitorService, MonitorService>();
    }
}
