using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using MonitorIsland.Services.Providers;

namespace MonitorIsland.Services
{
    /// <summary>
    /// 监控服务，统一管理所有监控提供器
    /// </summary>
    public class MonitorService : IMonitorService
    {
        private readonly ILogger<MonitorService> _logger;
        private readonly MonitorProviderFactory _providerFactory;
        private readonly CpuTemperatureProvider? _cpuTemperatureProvider;

        public MonitorService(
            ILogger<MonitorService> logger,
            MonitorProviderFactory providerFactory,
            IEnumerable<IMonitorProvider> providers)
        {
            _logger = logger;
            _providerFactory = providerFactory;
            
            // 获取 CPU 温度提供器的引用（用于传感器管理）
            _cpuTemperatureProvider = providers.OfType<CpuTemperatureProvider>().FirstOrDefault();
        }

        public float? GetMonitorValue(MonitorRequest request)
        {
            var provider = _providerFactory.GetProvider(request.MonitorType);
            if (provider == null)
            {
                _logger.LogWarning("未找到监控类型 {MonitorType} 对应的提供器", request.MonitorType);
                return null;
            }

            return provider.GetValue(request);
        }

        public List<CpuTemperatureSensorInfo> GetAvailableCpuTemperatureSensors()
        {
            if (_cpuTemperatureProvider == null)
            {
                _logger.LogWarning("CPU温度提供器不可用");
                return [];
            }

            return _cpuTemperatureProvider.GetAvailableSensors();
        }

        public void Dispose()
        {
            _providerFactory.Dispose();
            _logger.LogDebug("MonitorService 已释放资源");
        }
    }
}
