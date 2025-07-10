using MonitorIsland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="driveName">磁盘盘符（仅在监控磁盘空间时使用）</param>
        /// <returns>格式化后的监控值字符串。</returns>
        string GetFormattedMonitorValue(MonitorOption monitorType, string? driveName = null);

        /// <summary>
        /// 获取当前内存使用量（单位：MB）。
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
        /// <returns>CPU温度。</returns>
        float? GetCpuTemperature();

        /// <summary>
        /// 获取指定磁盘的剩余空间（单位：字节）。
        /// </summary>
        /// <param name="driveName">磁盘盘符。</param>
        /// <returns>磁盘剩余空间。</returns>
        float? GetDiskFreeSpace(string driveName);
    }
}
