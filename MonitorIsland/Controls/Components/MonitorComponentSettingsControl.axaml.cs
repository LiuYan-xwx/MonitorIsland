using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
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
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            // 更新 ID
            //if (!string.IsNullOrWhiteSpace(Settings.SelectedProvider?.Id) &&
            //    string.IsNullOrWhiteSpace(Settings.SelectedProviderId))
            //{
            //    Settings.SelectedProviderId = Settings.SelectedProvider!.Id;
            //}

            SyncSelectedProviderFromId();
            UpdateProviderSettingsControl();
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
                    SyncSelectedProviderFromId();
                    break;

                case nameof(Settings.SelectedProvider):
                    UpdateProviderSettingsControl();
                    break;
            }
        }

        private void SyncSelectedProviderFromId()
        {
            var id = Settings.SelectedProviderId;

            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            // 如果当前已经是该 Id 的独立快照，就不动它（保留其 Settings/SelectedUnit）
            if (Settings.SelectedProvider is { } current && current.Id == id)
                return;

            var template = MonitorProviders.FirstOrDefault(p => p.Id == id);
            if (template is null)
            {
                Logger.LogWarning("找不到监控提供方: {ProviderId}", id);
                return;
            }

            // 深拷贝
            Settings.SelectedProvider = template.CopyWithoutSettings();

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
