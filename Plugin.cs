using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Interfaces;
using MonitorIsland.Services;

namespace MonitorIsland;

/// <summary>
/// MonitorIsland 插件入口点，提供监控相关的组件和服务。
/// </summary>
[PluginEntrance]
public class Plugin : PluginBase
{
    /// <summary>
    /// 初始化插件，注册组件和服务。
    /// </summary>
    /// <param name="context">主机构建器上下文</param>
    /// <param name="services">服务集合</param>
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddComponent<MonitorComponent, MonitorComponentSettingsControl>();
        services.AddSingleton<IMonitorService, MonitorService>();
    }
}
