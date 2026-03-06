using System.ComponentModel;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 显示单位
    /// </summary>
    public enum DisplayUnit
    {
        [Description("MB")]
        MB,

        [Description("GB")]
        GB,

        [Description("TB")]
        TB,

        [Description("%")]
        Percent,

        [Description("°C")]
        Celsius,

        [Description("MB/s")]
        MBps,

        [Description("KB/s")]
        KBps,

        [Description("Mbps")]
        Mbps,

        [Description("Kbps")]
        Kbps
    }
}