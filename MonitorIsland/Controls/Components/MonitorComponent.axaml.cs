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
        private readonly SemaphoreSlim _updateLock = new(1, 1);

        public ILogger<MonitorComponent> Logger { get; }

        private IEnumerable<MonitorProvider> MonitorProviders => IMonitorService.MonitorProviders;

        public MonitorComponent(ILogger<MonitorComponent> logger, IMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimer_Ticked;
        }

        private async void OnTimer_Ticked(object? sender, EventArgs e)
        {
            await UpdateMonitorDataAsync();
        }

        /// <summary>
        /// 更新监控数据
        /// </summary>
        private async Task UpdateMonitorDataAsync()
        {
            if (!await _updateLock.WaitAsync(0))
            {
                return;
            }

            try
            {
                if (Settings.SelectedProviderBase == null)
                    return;

                var request = MonitorRequest.FromSelectedUnit(Settings.SelectedUnit);
                var value = await MonitorService.GetDataFromProviderAsync(Settings.SelectedProviderBase, request) ?? "N/A";

                if (double.TryParse(value, out var number))
                {
                    Settings.DisplayData = Math.Round(number, Settings.DecimalPlaces, MidpointRounding.AwayFromZero)
                                               .ToString($"F{Settings.DecimalPlaces}");
                }
                else
                {
                    Settings.DisplayData = value;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "更新监控数据时出现错误");
            }
            finally
            {
                _updateLock.Release();
            }
        }

        private void MonitorComponent_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            if (Settings.SelectedProvider is not null)
            {
                LoadProvider();
                Settings.SelectedProviderId = Settings.SelectedProvider.Id;
            }
            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
            if (Settings.SelectedProviderBase is IDisposable disposable)
            {
                disposable.Dispose();
            }
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

        private void LoadProvider()
        {
            if (Settings.SelectedProvider is null)
            {
                return;
            }
            var selected = Settings.SelectedProvider;

            var providerInstance = MonitorProviderBase.GetInstance(selected);
            if (providerInstance is null)
            {
                return;
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

            if (Settings.SelectedProviderBase is IDisposable oldDisposable)
            {
                oldDisposable.Dispose();
            }
            Settings.SelectedProviderBase = providerInstance;
        }

        private void ChangeProvider()
        {
            LoadProvider();
            if (Settings.SelectedProviderBase is not null)
            {
                Settings.DisplayPrefix = Settings.SelectedProviderBase.DefaultPrefix;
            }
        }
    }
}