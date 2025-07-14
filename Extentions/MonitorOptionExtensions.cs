using MonitorIsland.Models;

namespace MonitorIsland.Extensions
{
    public static class MonitorOptionExtensions
    {
        /// <summary>
        /// 获取按特定顺序排列的监控选项
        /// </summary>
        public static MonitorOption[] DisplayOrder => [
                MonitorOption.MemoryUsage,
                MonitorOption.MemoryUsageRate,
                MonitorOption.CpuUsage,
                MonitorOption.CpuTemperature,
                MonitorOption.DiskSpace
            ];
    }
}