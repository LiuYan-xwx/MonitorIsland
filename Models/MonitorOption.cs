namespace MonitorIsland.Models
{
    /// <summary>
    /// 定义可用的监控项目类型。
    /// </summary>
    public enum MonitorOption
    {
        /// <summary>
        /// 内存使用量
        /// </summary>
        MemoryUsage,
        /// <summary>
        /// 内存使用率
        /// </summary>
        MemoryUsageRate,
        /// <summary>
        /// CPU 利用率
        /// </summary>
        CpuUsage,
        /// <summary>
        /// CPU 温度
        /// </summary>
        CpuTemperature,
        /// <summary>
        /// 磁盘空间
        /// </summary>
        DiskSpace
    }
}
