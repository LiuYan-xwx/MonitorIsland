using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using Microsoft.Extensions.Logging;

namespace MonitorIsland.Services
{
    /// <summary>
    /// 监控提供器工厂
    /// </summary>
    public class MonitorProviderFactory : IDisposable
    {
        private readonly ILogger<MonitorProviderFactory> _logger;
        private readonly Dictionary<MonitorOption, IMonitorProvider> _providers = new();
        private readonly HashSet<IMonitorProvider> _allProviders = new();
        private int _disposed;

        public MonitorProviderFactory(
            ILogger<MonitorProviderFactory> logger,
            IEnumerable<IMonitorProvider> providers)
        {
            _logger = logger;

            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            _logger.LogInformation("提供器工厂初始化完成，已注册 {Count} 个提供器", _allProviders.Count);
        }

        /// <summary>
        /// 注册提供器
        /// </summary>
        private void RegisterProvider(IMonitorProvider provider)
        {
            _allProviders.Add(provider);

            foreach (var type in provider.SupportedTypes)
            {
                // 检测冲突
                if (_providers.ContainsKey(type))
                {
                    _logger.LogWarning(
                        "监控类型 {Type} 已被 {ExistingProvider} 注册，将被 {NewProvider} 覆盖",
                        type,
                        _providers[type].GetType().Name,
                        provider.GetType().Name
                    );
                }

                _providers[type] = provider;
                _logger.LogDebug("注册提供器: {Type} -> {ProviderName}", type, provider.GetType().Name);
            }
        }

        /// <summary>
        /// 获取指定类型的提供器
        /// </summary>
        public IMonitorProvider? GetProvider(MonitorOption type)
        {
            if (_providers.TryGetValue(type, out var provider))
            {
                return provider.IsAvailable ? provider : null;
            }

            _logger.LogWarning("未找到监控类型 {Type} 的提供器", type);
            return null;
        }

        /// <summary>
        /// 获取所有已注册的提供器
        /// </summary>
        public IEnumerable<IMonitorProvider> GetAllProviders() => _allProviders;

        /// <summary>
        /// 初始化所有提供器（预热）
        /// </summary>
        public void InitializeAll()
        {
            _logger.LogInformation("开始预热所有提供器");

            foreach (var provider in _allProviders)
            {
                try
                {
                    provider.Initialize();
                    _logger.LogDebug(
                        "提供器 {ProviderName} 预热完成，支持类型: {Types}",
                        provider.GetType().Name,
                        string.Join(", ", provider.SupportedTypes)
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "预热提供器 {ProviderName} 失败", provider.GetType().Name);
                }
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            foreach (var provider in _allProviders)
            {
                try
                {
                    provider.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "释放提供器 {ProviderName} 时出错", provider.GetType().Name);
                }
            }

            _providers.Clear();
            _allProviders.Clear();
            _logger.LogDebug("提供器工厂已释放");
        }
    }
}