using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;

namespace MonitorIsland.Extensions
{
    /// <summary>
    /// 监控提供方注册扩展方法
    /// </summary>
    public static class MonitorProviderRegistryExtensions
    {
        /// <summary>
        /// 注册监控提供方（无设置控件）
        /// </summary>
        /// <typeparam name="TProvider">监控提供方类型</typeparam>
        public static IServiceCollection AddMonitorProvider<TProvider>(this IServiceCollection services)
            where TProvider : MonitorProviderBase
        {
            var info = RegisterProviderInfo(typeof(TProvider));
            services.AddKeyedTransient<MonitorProviderBase, TProvider>(info.Id);
            return services;
        }

        /// <summary>
        /// 注册监控提供方（带设置控件）
        /// </summary>
        /// <typeparam name="TProvider">监控提供方类型</typeparam>
        /// <typeparam name="TSettingsControl">设置控件类型</typeparam>
        public static IServiceCollection AddMonitorProvider<TProvider, TSettingsControl>(this IServiceCollection services)
            where TProvider : MonitorProviderBase
            where TSettingsControl : MonitorProviderControlBase
        {
            var info = RegisterProviderInfo(typeof(TProvider));
            services.AddKeyedTransient<MonitorProviderBase, TProvider>(info.Id);
            services.AddKeyedTransient<MonitorProviderControlBase, TSettingsControl>(info.Id);
            return services;
        }

        private static MonitorProviderInfoAttribute RegisterProviderInfo(Type providerType)
        {
            if (providerType.GetCustomAttributes(false)
                .FirstOrDefault(x => x
                    is MonitorProviderInfoAttribute) is not MonitorProviderInfoAttribute info)
                throw new InvalidOperationException(
                    $"无法注册监控提供方 {providerType.FullName}：缺少 MonitorProviderInfoAttribute 特性");
            if (!IMonitorService.MonitorProviderInfos.TryAdd(info.Id, info))
                throw new InvalidOperationException(
                    $"无法注册监控提供方 {providerType.FullName}：Id {info.Id} 已被占用");
            IMonitorService.MonitorProviders.Add(new MonitorProvider
            {
                Id = info.Id,
                Name = info.Name
            });

            return info;
        }
    }
}
