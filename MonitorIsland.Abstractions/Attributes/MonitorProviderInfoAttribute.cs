using MonitorIsland.Abstractions.Models;

namespace MonitorIsland.Abstractions.Attributes;

/// <summary>
/// 监控提供方信息特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MonitorProviderInfoAttribute : Attribute
{
    /// <summary>
    /// 监控提供方 GUID
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 监控提供方名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 监控提供方描述
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 监控提供方支持的单位
    /// </summary>
    public IReadOnlyList<DisplayUnit>? AvailableUnits { get; }

    public MonitorProviderInfoAttribute(
        string id,
        string name,
        string description,
        DisplayUnit[] displayUnits)
    {
        Id = id;
        Name = name;
        Description = description;
        AvailableUnits = displayUnits;
    }

    public MonitorProviderInfoAttribute(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
