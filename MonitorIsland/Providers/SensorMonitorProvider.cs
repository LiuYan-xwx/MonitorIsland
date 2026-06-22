using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;
using MonitorIsland.Services;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.sensor",
        "传感器监控",
        "通过 LibreHardwareMonitor 监控任意硬件传感器",
        [DisplayUnit.Celsius])]
    public class SensorMonitorProvider : MonitorProviderBase<SensorMonitorSettings>
    {
        private readonly HardwareMonitorService _hardwareMonitorService;

        public SensorMonitorProvider(HardwareMonitorService hardwareMonitorService)
        {
            _hardwareMonitorService = hardwareMonitorService;
        }

        public override string DefaultPrefix => "传感器: ";

        public override string? GetData(MonitorRequest request)
        {
            if (Settings.SelectedSensor is null)
                return null;

            var sensorId = Settings.SelectedSensor.Identifier;

            var value = _hardwareMonitorService.GetSensorValue(sensorId);

            if (value == null)
                return "0";

            return value.ToString();
        }
    }
}
