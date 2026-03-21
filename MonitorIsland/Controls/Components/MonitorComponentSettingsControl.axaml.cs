using Avalonia;
using Avalonia.Threading;
using ClassIsland.Core;
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
            LoadProvider();
        }

        private void MonitorComponentSettingsControl_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private async void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.SelectedProviderId):
                    if (!string.IsNullOrWhiteSpace(Settings.SelectedProviderId))
                    {
                        await ChangeProviderAsync();
                    }
                    break;
            }
        }

        private void LoadProvider()
        {
            if (Settings.SelectedProvider is null)
                return;

            UpdateProviderSettingsControl();
        }

        private async Task ChangeProviderAsync()
        {
            var id = Settings.SelectedProviderId!;
            try
            {
                // 后台线程：只做查找/准备数据，不写 Settings
                var prepared = await Task.Run(() =>
                {
                    var template = MonitorProviders.FirstOrDefault(p => p.Id == id);
                    if (template is null)
                        return (Template: (MonitorProvider?)null, Units: (List<DisplayUnit>)[]);

                    if (!IMonitorService.MonitorProviderInfos.TryGetValue(id, out var info))
                        return (Template: template, Units: (List<DisplayUnit>)[]);

                    var units = info.AvailableUnits?.ToList() ?? [];
                    return (Template: template, Units: units);
                });

                if (prepared.Template is null)
                {
                    Logger.LogWarning("找不到监控提供方: {ProviderId}", id);
                    return;
                }

                if (!IMonitorService.MonitorProviderInfos.ContainsKey(id))
                {
                    Logger.LogWarning("找不到监控提供方信息: {ProviderId}", id);
                    return;
                }

                // UI 线程：写 Settings + 更新 UI
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Settings.SelectedProvider = prepared.Template.CopyWithoutSettings();
                    Settings.AvailableUnits = prepared.Units;
                    Settings.SelectedUnit = Settings.AvailableUnits.FirstOrDefault();
                    UpdateProviderSettingsControl();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "切换监控提供方时出现错误: {ProviderId}", id);
            }
        }

        private void UpdateProviderSettingsControl()
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Settings.SelectedProvider is null)
                {
                    Unload();
                    return;
                }
                UpdateContent();
            });
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
