using MonitorIsland.Models;

namespace MonitorIsland.Interfaces
{
    /// <summary>
    /// 提供监控相关服务的接口。
    /// </summary>
    public interface IMonitorService : IDisposable
    {
        /// <summary>
        /// 获取指定监控类型格式化后的值。
        /// </summary>
        /// <param name="monitorType">监控类型枚举。</param>
        /// <param name="unit">显示单位</param>
        /// <param name="driveName">磁盘盘符（仅在监控磁盘空间时使用）</param>
        /// <param name="cpuTemperatureSensorId">CPU温度传感器ID（仅在监控CPU温度时使用）</param>
        /// <returns>格式化后的监控值字符串。</returns>
        string GetFormattedMonitorValue(MonitorOption monitorType, DisplayUnit unit, string? driveName = null, string? cpuTemperatureSensorId = null);

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
