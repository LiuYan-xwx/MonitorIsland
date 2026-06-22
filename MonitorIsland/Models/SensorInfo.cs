using LibreHardwareMonitor.Hardware;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 传感器信息
    /// </summary>
    public class SensorInfo
    {
        /// <summary>
        /// 传感器的唯一标识符（LibreHardwareMonitor Identifier）
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// 传感器名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 硬件名称
        /// </summary>
        public string HardwareName { get; set; } = string.Empty;

        /// <summary>
        /// 传感器类型（Temperature、Load、Clock、Fan、Voltage、Power 等）
        /// </summary>
        public required SensorType SensorType { get; set; }

        /// <summary>
        /// 显示文本（硬件名称 - 传感器名称）
        /// </summary>
        public string DisplayText => $"{HardwareName} - {Name}";

        public override bool Equals(object? obj)
        {
            return obj is SensorInfo other && Identifier == other.Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}
