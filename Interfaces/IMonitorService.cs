using MonitorIsland.Models;

namespace MonitorIsland.Interfaces
{
    /// <summary>
    /// 提供监控相关服务的接口。
    /// </summary>
    public interface IMonitorService : IDisposable
    {
        /// <summary>
        /// 获取指定监控类型的原始值。
        /// </summary>
        /// <param name="request">监控请求参数</param>
        /// <returns>监控值（单位：字节、百分比或摄氏度）</returns>
        float? GetMonitorValue(MonitorRequest request);

        /// <summary>
        /// 获取所有可用的CPU温度传感器列表
        /// </summary>
        /// <returns>CPU温度传感器信息列表</returns>
        List<CpuTemperatureSensorInfo> GetAvailableCpuTemperatureSensors();
    }
}
