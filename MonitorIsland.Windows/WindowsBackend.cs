using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Windows.Services;

namespace MonitorIsland.Windows;

public sealed class WindowsBackend : IMonitorIslandBackend
{
    public void Initialize(IServiceCollection services)
    {
        services.AddSingleton<IHardwareMonitorService, WindowsHardwareMonitorService>();
    }
}
