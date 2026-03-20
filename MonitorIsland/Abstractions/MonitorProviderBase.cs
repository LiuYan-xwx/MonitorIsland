using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using MonitorIsland.Models;
using System.Text.Json;

namespace MonitorIsland.Abstractions
{
    /// <summary>
    /// 监控提供方基类
    /// </summary>
    public abstract class MonitorProviderBase : IDisposable
    {
        /// <summary>
        /// 获取监控值
        /// </summary>
        public abstract string? GetData(MonitorRequest request);

        internal object? SettingsInternal { get; set; }

        /// <summary>
        /// 默认前缀
        /// </summary>
        public virtual string DefaultPrefix => string.Empty;

        /// <summary>
        /// 获取监控提供方实例
        /// </summary>
        public static MonitorProviderBase? GetInstance(MonitorProvider? monitorProvider)
        {
            if (string.IsNullOrEmpty(monitorProvider?.Id)) return null;

            var provider = IAppHost.Host?.Services.GetKeyedService<MonitorProviderBase>(monitorProvider.Id);
            if (provider == null)
            {
                return null;
            }

            var settingsType = provider.GetType().BaseType?.GetGenericArguments().FirstOrDefault();
            if (settingsType != null)
            {
                if (monitorProvider.Settings is JsonElement json)
                    monitorProvider.Settings = json.Deserialize(settingsType);
                if (monitorProvider.Settings?.GetType() != settingsType)
                    monitorProvider.Settings = Activator.CreateInstance(settingsType);
            }
            provider.SettingsInternal = monitorProvider.Settings;

            return provider;
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
        }
    }

    /// <inheritdoc cref="MonitorProviderBase"/>
    /// <typeparam name="TSettings">提供方设置类型</typeparam>
    public abstract class MonitorProviderBase<TSettings> : MonitorProviderBase where TSettings : class
    {
        /// <summary>
        /// 当前提供方的设置
        /// </summary>
        protected TSettings Settings => (SettingsInternal as TSettings)!;
    }
}
