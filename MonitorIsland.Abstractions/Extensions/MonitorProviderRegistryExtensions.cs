using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Abstractions.Attributes;
using MonitorIsland.Abstractions.Controls;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Abstractions.Providers;

namespace MonitorIsland.Abstractions.Extensions;

/// <summary>
/// 监控提供方注册扩展方法
/// </summary>
public static class MonitorProviderRegistryExtensions
{
    /// <summary>
    /// 注册监控提供方（无设置控件）
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    public static IServiceCollection AddMonitorProvider<TProvider>(
        this IServiceCollection services)
        where TProvider : MonitorProviderBase
    {
        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase, TProvider>(info.Id);
        return services;
    }

    /// <summary>
    /// 使用工厂方法注册监控提供方（无设置控件）
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="implementationFactory">用于创建监控提供方的工厂方法</param>
    public static IServiceCollection AddMonitorProvider<TProvider>(
        this IServiceCollection services,
        Func<IServiceProvider, TProvider> implementationFactory)
        where TProvider : MonitorProviderBase
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);

        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase>(info.Id,
            (serviceProvider, _) => implementationFactory(serviceProvider));
        return services;
    }

    /// <summary>
    /// 注册监控提供方（带设置控件）
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    /// <typeparam name="TSettingsControl">设置控件类型</typeparam>
    public static IServiceCollection AddMonitorProvider<TProvider, TSettingsControl>(
        this IServiceCollection services)
        where TProvider : MonitorProviderBase
        where TSettingsControl : MonitorProviderControlBase
    {
        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase, TProvider>(info.Id);
        services.AddKeyedTransient<MonitorProviderControlBase, TSettingsControl>(info.Id);
        return services;
    }

    /// <summary>
    /// 使用工厂方法注册监控提供方（带设置控件）
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    /// <typeparam name="TSettingsControl">设置控件类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="implementationFactory">用于创建监控提供方的工厂方法</param>
    public static IServiceCollection AddMonitorProvider<TProvider, TSettingsControl>(
        this IServiceCollection services,
        Func<IServiceProvider, TProvider> implementationFactory)
        where TProvider : MonitorProviderBase
        where TSettingsControl : MonitorProviderControlBase
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);

        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase>(info.Id,
            (serviceProvider, _) => implementationFactory(serviceProvider));
        services.AddKeyedTransient<MonitorProviderControlBase, TSettingsControl>(info.Id);
        return services;
    }

    /// <summary>
    /// 使用工厂方法注册设置控件
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    /// <typeparam name="TSettingsControl">设置控件类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="settingsControlFactory">用于创建设置控件的工厂方法</param>
    public static IServiceCollection AddMonitorProvider<TProvider, TSettingsControl>(
        this IServiceCollection services,
        Func<IServiceProvider, TSettingsControl> settingsControlFactory)
        where TProvider : MonitorProviderBase
        where TSettingsControl : MonitorProviderControlBase
    {
        ArgumentNullException.ThrowIfNull(settingsControlFactory);

        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase, TProvider>(info.Id);
        services.AddKeyedTransient<MonitorProviderControlBase>(info.Id,
            (serviceProvider, _) => settingsControlFactory(serviceProvider));
        return services;
    }

    /// <summary>
    /// 使用工厂方法注册监控提供方和设置控件
    /// </summary>
    /// <typeparam name="TProvider">监控提供方类型</typeparam>
    /// <typeparam name="TSettingsControl">设置控件类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="implementationFactory">用于创建监控提供方的工厂方法</param>
    /// <param name="settingsControlFactory">用于创建设置控件的工厂方法</param>
    public static IServiceCollection AddMonitorProvider<TProvider, TSettingsControl>(
        this IServiceCollection services,
        Func<IServiceProvider, TProvider> implementationFactory,
        Func<IServiceProvider, TSettingsControl> settingsControlFactory)
        where TProvider : MonitorProviderBase
        where TSettingsControl : MonitorProviderControlBase
    {
        ArgumentNullException.ThrowIfNull(implementationFactory);
        ArgumentNullException.ThrowIfNull(settingsControlFactory);

        var info = RegisterProviderInfo(typeof(TProvider));
        services.AddKeyedTransient<MonitorProviderBase>(info.Id,
            (serviceProvider, _) => implementationFactory(serviceProvider));
        services.AddKeyedTransient<MonitorProviderControlBase>(info.Id,
            (serviceProvider, _) => settingsControlFactory(serviceProvider));
        return services;
    }

    private static MonitorProviderInfoAttribute RegisterProviderInfo(Type providerType)
    {
        if (providerType.GetCustomAttributes(false)
                .FirstOrDefault(x => x is MonitorProviderInfoAttribute)
            is not MonitorProviderInfoAttribute info)
        {
            throw new InvalidOperationException(
                $"无法注册监控提供方 {providerType.FullName}：缺少 " +
                $"{nameof(MonitorProviderInfoAttribute)} 特性");
        }

        if (!IMonitorService.MonitorProviderInfos.TryAdd(info.Id, info))
        {
            throw new InvalidOperationException(
                $"无法注册监控提供方 {providerType.FullName}：Id {info.Id} 已被占用");
        }

        IMonitorService.MonitorProviders.Add(new MonitorProvider
        {
            Id = info.Id,
            Name = info.Name
        });

        return info;
    }
}
