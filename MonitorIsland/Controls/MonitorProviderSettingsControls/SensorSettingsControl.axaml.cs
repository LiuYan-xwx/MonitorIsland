using ClassIsland.Core.Abstractions.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Controls;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Abstractions.Models;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;
using System.Collections.ObjectModel;

namespace MonitorIsland.Controls.MonitorProviderSettingsControls;

public partial class SensorSettingsControl : MonitorProviderControlBase<SensorMonitorSettings>
{
    private readonly ILogger<SensorSettingsControl> _logger;
    private readonly IHardwareMonitorService _hardwareMonitorService;

    public ObservableCollection<SensorTreeNode> SensorTreeNodes { get; } = [];

    public SensorSettingsControl(
        ILogger<SensorSettingsControl> logger,
        IHardwareMonitorService hardwareMonitorService)
    {
        InitializeComponent();
        _logger = logger;
        _hardwareMonitorService = hardwareMonitorService;
    }

    private void MonitorProviderControlBase_Loaded(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        UpdateSelectedSensorDisplay();
    }

    private async Task LoadAvailableSensors()
    {
        try
        {
            SensorTreeNodes.Clear();
            if (!_hardwareMonitorService.IsSupported)
            {
                _logger.LogInformation("当前平台不支持硬件传感器");
                return;
            }

            if (!_hardwareMonitorService.IsReady)
                await _hardwareMonitorService.ReadyTask;

            foreach (var group in _hardwareMonitorService.GetSensorGroups())
            {
                var hardwareNode = new SensorTreeNode
                {
                    DisplayName = group.Name,
                    HardwareKind = group.HardwareKind
                };

                foreach (var sensor in group.Sensors)
                {
                    hardwareNode.Children.Add(new SensorTreeNode
                    {
                        DisplayName = sensor.Name,
                        IsSensor = true,
                        Sensor = sensor
                    });
                }

                SensorTreeNodes.Add(hardwareNode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载可用传感器时发生错误");
        }
    }

    private void OnSensorSelected(object? sender, SensorInfo? sensor)
    {
        if (sensor is null)
            return;

        Settings.SelectedSensor = sensor;
        UpdateSelectedSensorDisplay();
    }

    private void UpdateSelectedSensorDisplay()
    {
        SelectedSensorText.Text = Settings.SelectedSensor?.DisplayText ?? string.Empty;
    }

    [RelayCommand]
    private async Task SelectSensor()
    {
        await LoadAvailableSensors();
        var panel = new SensorTreePanel();
        panel.SetData(SensorTreeNodes, Settings.SelectedSensor);
        panel.SensorSelected += OnSensorSelected;
        SettingsPageBase.OpenDrawerCommand.Execute(panel);
    }
}
