using MonitorIsland.Models;

namespace MonitorIsland.Interfaces
{
    /// <summary>
    /// 监控数据提供器接口
    /// </summary>
    public interface IMonitorProvider : IDisposable
    {
        /// <summary>
        /// 该提供器支持的监控类型列表
        /// </summary>
        IReadOnlyList<MonitorOption> SupportedTypes { get; }

        /// <summary>
        /// 获取监控值
        /// </summary>
        /// <param name="request">监控请求，包含必要的参数</param>
        /// <returns>监控值，失败返回 null</returns>
        float? GetValue(MonitorRequest request);

        /// <summary>
        /// 初始化提供器（可选，用于预热）
        /// </summary>
        void Initialize();

        /// <summary>
        /// 提供器是否可用
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 检查是否支持指定的监控类型
        /// </summary>
        /// <param name="type">监控类型</param>
        /// <returns>是否支持</returns>
        bool Supports(MonitorOption type) => SupportedTypes.Contains(type);
    }
}