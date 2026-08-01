using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Controls;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Models.ComponentSettings;
using System.ComponentModel;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponentSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorComponentSettingsControl : ComponentBase<MonitorComponentSettings>
    {
        public ILogger<MonitorComponentSettingsControl> Logger { get; }
        public List<MonitorProvider> MonitorProviders => IMonitorService.MonitorProviders;

        public MonitorComponentSettingsControl(ILogger<MonitorComponentSettingsControl> logger)
        {
            Logger = logger;
            InitializeComponent();
        }

        private void MonitorComponentSettingsControl_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Settings.TryMigrateFromOldSettings();
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            LoadProvider();
        }

        private void MonitorComponentSettingsControl_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.SelectedProviderId):
                    if (!string.IsNullOrWhiteSpace(Settings.SelectedProviderId))
                    {
                        ChangeProvider();
                    }
                    break;
            }
        }

        private void LoadProvider()
        {
            if (Settings.SelectedProvider is null)
            {
                return;
            }

            var id = ResolveProviderId();
            if (string.IsNullOrWhiteSpace(id))
            {
                Logger.LogWarning("无法加载监控提供方：ProviderId 为空");
                return;
            }
            if (!IMonitorService.MonitorProviderInfos.TryGetValue(id, out var providerInfo))
            {
                Logger.LogWarning("无法加载监控提供方：找不到 ProviderInfo {ProviderId}", id);
                return;
            }

            Settings.SelectedProviderId = id;
            var availableUnits = providerInfo.AvailableUnits;
            Settings.AvailableUnits = availableUnits?.ToList() ?? [];
            UpdateProviderSettingsControl();
        }

        private void ChangeProvider()
        {
            var id = ResolveProviderId();
            if (string.IsNullOrWhiteSpace(id))
            {
                Logger.LogWarning("找不到监控提供方: ProviderId 为空");
                return;
            }

            var template = MonitorProviders.FirstOrDefault(p => p.Id == id);
            if (template is null)
            {
                Logger.LogWarning("找不到监控提供方: {ProviderId}", id);
                return;
            }

            // 深拷贝
            Settings.SelectedProvider = template.CopyWithoutSettings();
            Settings.SelectedProviderId = id;

            if (!IMonitorService.MonitorProviderInfos.TryGetValue(id, out var providerInfo))
            {
                Logger.LogWarning("找不到监控提供方信息: {ProviderId}", id);
                return;
            }

            var availableUnits = providerInfo.AvailableUnits;
            Settings.AvailableUnits = availableUnits?.ToList() ?? [];
            Settings.SelectedUnit = Settings.AvailableUnits.FirstOrDefault();
            UpdateProviderSettingsControl();
        }

        private string? ResolveProviderId()
        {
            return !string.IsNullOrWhiteSpace(Settings.SelectedProviderId)
                ? Settings.SelectedProviderId
                : Settings.SelectedProvider?.Id;
        }

        private void UpdateProviderSettingsControl()
        {
            if (Settings.SelectedProvider is null)
            {
                Unload();
                return;
            }
            UpdateContent();
        }

        private void UpdateContent()
        {
            var newControl = MonitorProviderControlBase.GetInstance(Settings.SelectedProvider);
            if (newControl != null)
            {
                ProviderSettingsControl.Content = newControl;
                Settings.ShowProviderSettingsControl = true;
            }
            else
            {
                Unload();
            }
        }

        private void Unload()
        {
            ProviderSettingsControl.Content = null;
            Settings.ShowProviderSettingsControl = false;
        }
    }
}
