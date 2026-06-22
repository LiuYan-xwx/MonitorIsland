using ClassIsland.Core.Abstractions.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;
using MonitorIsland.Services;
using System.Collections.ObjectModel;

namespace MonitorIsland.Controls.MonitorProviderSettingsControls;

public partial class SensorSettingsControl : MonitorProviderControlBase<SensorMonitorSettings>
{
    private readonly ILogger<SensorSettingsControl> _logger;
    private readonly HardwareMonitorService _hardwareMonitorService;

    public ObservableCollection<SensorTreeNode> SensorTreeNodes { get; } = [];

    public SensorSettingsControl(ILogger<SensorSettingsControl> logger,
                                 HardwareMonitorService hardwareMonitorService)
    {
        InitializeComponent();
        _logger = logger;
        _hardwareMonitorService = hardwareMonitorService;
    }

    private void MonitorProviderControlBase_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        UpdateSelectedSensorDisplay();
    }

    private async Task LoadAvailableSensors()
    {
        try
        {
            SensorTreeNodes.Clear();
            if (_hardwareMonitorService.IsReady == false)
            {
                await _hardwareMonitorService.ReadyTask;
            }
            var treeNodes = _hardwareMonitorService.GetSensorTree();
            foreach (var node in treeNodes)
            {
                SensorTreeNodes.Add(node);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载可用传感器时发生错误");
        }
    }

    private void OnSensorSelected(object? sender, SensorInfo? sensor)
    {
        if (sensor != null)
        {
            Settings.SelectedSensor = sensor;
            UpdateSelectedSensorDisplay();
        }
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
