using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Attributes;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Abstractions.Providers;
using System.Reflection;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly ILogger<MonitorService> Logger = logger;

        public async Task<MonitorDataResult> GetDataFromProviderAsync(MonitorProviderBase providerInstance, MonitorRequest request, CancellationToken cancellationToken = default)
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
                return MonitorDataResult.Error("错误，请查看日志");
            }
        }
    }
}
