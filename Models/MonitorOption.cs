using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// CPU 利用率
        /// </summary>
        CpuUsage,
        /// <summary>
        /// CPU 温度
        /// </summary>
        CpuTemperature
    }
}
