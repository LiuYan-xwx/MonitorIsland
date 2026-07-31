using Avalonia.Controls;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Abstractions.Models;
using System.Text.Json;

namespace MonitorIsland.Abstractions.Controls;

/// <summary>
/// 监控提供方设置控件基类
/// </summary>
public abstract class MonitorProviderControlBase : UserControl
{
    internal object? SettingsInternal { get; set; }

    internal bool IsNewAdded = false;

    /// <summary>
    /// 获取监控设置控件实例。
    /// </summary>
    /// <param name="monitorProvider">要获取监控设置控件的监控项。</param>
    public static MonitorProviderControlBase? GetInstance(
        MonitorProvider? monitorProvider)
    {
        if (string.IsNullOrEmpty(monitorProvider?.Id))
            return null;

        // Bug：过于简单的控件会在此开始加载 AXAML，此时 Settings 仍为 null。
        var control = IAppHost.Host?.Services
            .GetKeyedService<MonitorProviderControlBase>(monitorProvider.Id);
        if (control is null)
            return null;

        var settingsType = control.GetType().BaseType?
            .GetGenericArguments()
            .FirstOrDefault();
        if (settingsType is not null)
        {
            if (monitorProvider.Settings is JsonElement json)
                monitorProvider.Settings = json.Deserialize(settingsType);

            if (monitorProvider.Settings?.GetType() != settingsType)
                monitorProvider.Settings = Activator.CreateInstance(settingsType);
        }

        control.SettingsInternal = monitorProvider.Settings;
        return control;
    }
}

public abstract class MonitorProviderControlBase<TSettings> : MonitorProviderControlBase
    where TSettings : class
{
    /// <summary>
    /// 当前监控项的设置。注意：请勿在构造函数中访问。
    /// </summary>
    protected TSettings Settings =>
        SettingsInternal as TSettings ??
        throw new ArgumentNullException(
            nameof(Settings),
            $"过早访问监控项设置（{typeof(TSettings).FullName}）。");
}
