using System.ComponentModel;

namespace MonitorIsland.Abstractions.Models;

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

    [Description("自动")]
    Auto
}
