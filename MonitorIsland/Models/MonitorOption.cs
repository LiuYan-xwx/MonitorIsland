using System.ComponentModel;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 定义可用的监控项目类型。
    /// </summary>
    [Obsolete("仅用于旧版本设置迁移，请使用 Provider 架构（MonitorIsland.Providers）。")]
    public enum MonitorOption
    {
        /// <summary>
        /// 内存使用量
        /// </summary>
        [Description("内存使用量")]
        MemoryUsage,

        /// <summary>
        /// CPU 利用率
        /// </summary>
        [Description("CPU 利用率")]
        CpuUsage,

        /// <summary>
        /// CPU 温度
        /// </summary>
        [Description("CPU 温度")]
        CpuTemperature,

        /// <summary>
        /// 磁盘空间
        /// </summary>
        [Description("磁盘空间")]
        DiskSpace,

        /// <summary>
        /// 内存使用率
        /// </summary>
        [Description("内存使用率")]
        MemoryUsageRate
    }
}
