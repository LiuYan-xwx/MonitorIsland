using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using System.Reflection;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly ILogger<MonitorService> Logger = logger;

        public async Task<string?> GetDataFromProviderAsync(MonitorProviderBase providerInstance, MonitorRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(providerInstance);
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                return await Task.Run(() => providerInstance.GetData(request), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Logger.LogInformation("从提供器 {ProviderName} 获取数据的任务已取消", providerInstance.GetType().GetCustomAttribute<MonitorProviderInfoAttribute>()?.Name);
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "从提供器 {ProviderName} 获取数据时出现错误", providerInstance.GetType().GetCustomAttribute<MonitorProviderInfoAttribute>()?.Name);
                return null;
            }
        }
    }
}
