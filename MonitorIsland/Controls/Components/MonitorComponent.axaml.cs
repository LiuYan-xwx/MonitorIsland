using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
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
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
        "AE533FE2-A53F-4104-8C38-37DA018A98BB",
        "监控",
        "\uEE21",
        "监控您电脑的各种信息"
    )]
    public partial class MonitorComponent : ComponentBase<MonitorComponentSettings>
    {
        private readonly DispatcherTimer _timer;
        private readonly IMonitorService MonitorService;

        public ILogger<MonitorComponent> Logger { get; }

        private List<MonitorProvider> MonitorProviders => IMonitorService.MonitorProviders;

        public MonitorComponent(ILogger<MonitorComponent> logger, IMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimer_Ticked;
        }

        private void OnTimer_Ticked(object? sender, EventArgs e)
        {
            UpdateMonitorData();
        }

        /// <summary>
        /// 更新监控数据
        /// </summary>
        private async void UpdateMonitorData()
        {
            if (Settings.SelectedProviderBase == null)
            {
                Logger.LogWarning("没有选择监控提供方");
                return;
            }
            var value = await MonitorService.GetDataFromProviderAsync(Settings.SelectedProviderBase);
            Settings.DisplayData = value ?? "N/A";
        }

        private void MonitorComponent_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            if (Settings.SelectedProvider is not null)
            {
                ChangeProvider();
                Settings.SelectedProviderId = Settings.SelectedProvider.Id;
            }
            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
            Settings.SelectedProviderBase?.Dispose();
            Settings.SelectedProviderBase = null;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.RefreshInterval):
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    break;
                case nameof(Settings.SelectedProvider):
                    ChangeProvider();
                    break;
            }
        }

        private void ChangeProvider()
        {
            if (Settings.SelectedProvider is null)
            {
                return;
            }

            var selected = Settings.SelectedProvider;
            var oldProvider = Settings.SelectedProviderBase;

            var providerInstance = MonitorProviderBase.GetInstance(selected);
            if (providerInstance is null)
            {
                return;
            }

            if (oldProvider is not null && !ReferenceEquals(oldProvider, providerInstance))
            {
                oldProvider.Dispose();
            }

            var baseType = providerInstance.GetType().BaseType;
            if (baseType is not null
                && baseType.IsGenericType
                && baseType.GetGenericTypeDefinition() == typeof(MonitorProviderBase<>))
            {
                var settingsType = baseType.GetGenericArguments()[0];
                var settings = selected.Settings;
                if (settings?.GetType() != settingsType)
                {
                    settings = Activator.CreateInstance(settingsType);
                }
                selected.Settings = settings;
            }
            var availableUnits = IMonitorService.MonitorProviderInfos[selected.Id].AvailableUnits;
            providerInstance.SelectedUnit = selected.SelectedUnit;
            Settings.SelectedProviderBase = providerInstance;
            Settings.AvailableUnits = availableUnits?.ToList() ?? [];
            Settings.SelectedUnit = selected.SelectedUnit;
            Settings.DisplayPrefix = providerInstance.DefaultPrefix;
        }
    }
}
