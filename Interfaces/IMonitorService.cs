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
        /// 获取当前内存使用量（单位：字节）。
        /// </summary>
        /// <returns>内存使用量。</returns>
        float? GetMemoryUsage();

        /// <summary>
        /// 获取当前CPU利用率（百分比）。
        /// </summary>
        /// <returns>CPU利用率。</returns>
        float? GetCpuUsage();

        /// <summary>
        /// 获取当前CPU温度（单位：摄氏度）。
        /// </summary>
        /// <param name="sensorId">指定的传感器ID</param>
        /// <returns>CPU温度。</returns>
        float? GetCpuTemperature(string? sensorId = null);

        /// <summary>
        /// 获取指定磁盘的剩余空间（单位：字节）。
        /// </summary>
        /// <param name="driveName">磁盘盘符。</param>
        /// <returns>磁盘剩余空间。</returns>
        float? GetDiskFreeSpace(string driveName);

        /// <summary>
        /// 获取所有可用的CPU温度传感器列表
        /// </summary>
        /// <returns>CPU温度传感器信息列表</returns>
        List<CpuTemperatureSensorInfo> GetAvailableCpuTemperatureSensors();
    }
}
