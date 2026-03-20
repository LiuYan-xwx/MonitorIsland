using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;

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
        public static readonly Dictionary<string, MonitorProviderInfoAttribute> MonitorProviderInfos = [];

        public static readonly List<MonitorProvider> MonitorProviders = [];

        public Task<string?> GetDataFromProviderAsync(MonitorProviderBase monitorProvider, MonitorRequest request);
    }
}
