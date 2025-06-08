using System;

namespace MonitorIsland.Models
{
    public class HardwareInfo
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string HardwareName { get; set; } = string.Empty;
        public string SensorName { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public override string ToString() => DisplayName;

        public override bool Equals(object? obj)
        {
            return obj is HardwareInfo other && Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}