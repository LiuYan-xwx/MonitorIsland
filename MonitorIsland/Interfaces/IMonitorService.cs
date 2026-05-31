using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using System.Collections.Concurrent;

namespace MonitorIsland.Interfaces
{
    /// <summary>
    /// 提供监控相关服务的接口。
    /// </summary>
    public interface IMonitorService
    {
        /// <summary>
        /// 所有监控提供方信息。
        /// 键为监控提供方 ID，值为监控提供方信息。
        /// </summary>
        public static readonly ConcurrentDictionary<string, MonitorProviderInfoAttribute> MonitorProviderInfos = new();

        public static readonly ConcurrentBag<MonitorProvider> MonitorProviders = [];

        public Task<string?> GetDataFromProviderAsync(MonitorProviderBase monitorProvider, MonitorRequest request);
    }
}
