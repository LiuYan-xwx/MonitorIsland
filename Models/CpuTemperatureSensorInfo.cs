namespace MonitorIsland.Models
{
    /// <summary>
    /// CPU温度传感器信息
    /// </summary>
    public class CpuTemperatureSensorInfo
    {
        /// <summary>
        /// 传感器唯一标识符
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 传感器名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 硬件名称
        /// </summary>
        public string HardwareName { get; set; } = string.Empty;

        /// <summary>
        /// 显示文本（硬件名称 - 传感器名称）
        /// </summary>
        public string DisplayText => $"{HardwareName} - {Name}";

        public override bool Equals(object? obj)
        {
            return obj is CpuTemperatureSensorInfo other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}