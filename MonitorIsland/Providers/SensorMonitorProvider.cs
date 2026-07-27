using MonitorIsland.Abstractions;
using MonitorIsland.Abstractions.Interfaces;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;

namespace MonitorIsland.Providers;

[MonitorProviderInfo(
    "monitorisland.sensor",
    "传感器监控",
    "通过 LibreHardwareMonitor 监控任意硬件传感器",
    [DisplayUnit.Celsius])]
public class SensorMonitorProvider(IHardwareMonitorService hardwareMonitorService)
    : MonitorProviderBase<SensorMonitorSettings>
{
    public override string DefaultPrefix => "传感器: ";

    public override MonitorDataResult GetData(MonitorRequest request)
    {
        if (!hardwareMonitorService.IsSupported)
            return MonitorDataResult.Error("当前平台不支持硬件传感器");

        if (Settings.SelectedSensor is null)
            return MonitorDataResult.Error("未选择传感器");

        var value = hardwareMonitorService.GetSensorValue(Settings.SelectedSensor.Identifier);
        if (value is null)
            return MonitorDataResult.Error("传感器当前不可用");

        return MonitorDataResult.Success(value.Value.ToString(), DisplayUnit.Celsius);
    }
}
