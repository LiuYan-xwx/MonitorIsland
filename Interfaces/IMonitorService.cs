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
        /// 获取指定监控类型的格式化监控值。
        /// </summary>
        /// <param name="monitorType">
        /// 监控类型：
        /// 0 - 内存使用量，
        /// 1 - CPU 利用率，
        /// 2 - CPU 温度。
        /// </param>
        /// <returns>格式化后的监控值字符串。</returns>
        string GetFormattedMonitorValue(int monitorType);

        /// <summary>
        /// 获取当前内存使用量（单位：MB）。
        /// </summary>
        /// <returns>内存使用量。</returns>
        float GetMemoryUsage();

        /// <summary>
        /// 获取当前CPU利用率（百分比）。
        /// </summary>
        /// <returns>CPU利用率。</returns>
        float GetCpuUsage();

        /// <summary>
        /// 获取当前CPU温度（单位：摄氏度）。
        /// </summary>
        /// <returns>CPU温度。</returns>
        float GetCpuTemperature();
    }
}
