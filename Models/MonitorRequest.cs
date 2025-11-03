namespace MonitorIsland.Models
{
    /// <summary>
    /// 封装监控请求参数
    /// </summary>
    public class MonitorRequest
    {
        /// <summary>
        /// 监控类型
        /// </summary>
        public MonitorOption MonitorType { get; set; }

        /// <summary>
        /// 磁盘盘符（仅在监控磁盘空间时使用）
        /// </summary>
        public string? DriveName { get; set; }

        /// <summary>
        /// CPU温度传感器ID（仅在监控CPU温度时使用）
        /// </summary>
        public string? CpuTemperatureSensorId { get; set; }

        /// <summary>
        /// 显示单位
        /// </summary>
        public DisplayUnit Unit { get; set; }
    }
}
