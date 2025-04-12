
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorIsland.Controls.Components;
using MonitorIsland.Services;

namespace MonitorIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddComponent<MonitorComponent, MonitorComponentSettingsControl>();
        services.AddSingleton<MonitorService>();
        AppBase.Current.AppStarted += CurrentOnAppStarted;
    }
    private void CurrentOnAppStarted(object? sender, EventArgs e)
    {
        IAppHost.GetService<MonitorService>();
    }
}
