using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Interfaces;
using System.Reflection;

namespace MonitorIsland.Services
{
    public class MonitorService(ILogger<MonitorService> logger) : IMonitorService
    {
        private readonly ILogger<MonitorService> Logger = logger;

        public Task<string?> GetDataFromProviderAsync(MonitorProviderBase providerInstance)
        {
            try
            {
                var value = providerInstance.GetData();
                return Task.FromResult<string?>(value);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "从提供器 {ProviderName} 获取数据时出现错误", providerInstance.GetType()?.GetCustomAttribute<MonitorProviderInfoAttribute>()?.Name);
                return Task.FromResult<string?>(null);
            }
        }
    }
}
