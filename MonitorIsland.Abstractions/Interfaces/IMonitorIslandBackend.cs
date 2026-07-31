using Microsoft.Extensions.DependencyInjection;

namespace MonitorIsland.Abstractions.Interfaces;

public interface IMonitorIslandBackend
{
    void Initialize(IServiceCollection services);
}
