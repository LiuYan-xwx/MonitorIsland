using MonitorIsland.Models;

namespace MonitorIsland.Services
{
    /// <summary>
    /// 监控值格式化服务
    /// </summary>
    public class MonitorValueFormatter
    {
        // 字节转换常量 - 使用逐级定义以提高可读性和可维护性
        private const float BytesToKB = 1024f;
        private const float BytesToMB = BytesToKB * 1024f;
        private const float BytesToGB = BytesToMB * 1024f;
        private const float BytesToTB = BytesToGB * 1024f;

        /// <summary>
        /// 根据监控类型和单位格式化监控值
        /// </summary>
        /// <param name="value">原始监控值</param>
        /// <param name="monitorType">监控类型</param>
        /// <param name="unit">显示单位</param>
        /// <returns>格式化后的字符串</returns>
        public static string Format(float? value, MonitorOption monitorType, DisplayUnit unit)
        {
            if (!value.HasValue)
                return "N/A";

            var format = GetFormatString(monitorType);
            var (convertedValue, unitString) = ConvertValue(value.Value, unit);

            return string.IsNullOrEmpty(format)
                ? $"{convertedValue} {unitString}"
                : $"{convertedValue.ToString(format)} {unitString}";
        }

        private static string GetFormatString(MonitorOption monitorType)
        {
            return monitorType switch
            {
                MonitorOption.MemoryUsage => "F1",
                MonitorOption.MemoryUsageRate => "F2",
                MonitorOption.CpuUsage => "F2",
                MonitorOption.CpuTemperature => "F1",
                MonitorOption.DiskSpace => "F1",
                _ => ""
            };
        }

        private static (float, string) ConvertValue(float value, DisplayUnit unit)
        {
            return unit switch
            {
                DisplayUnit.MB => (value / BytesToMB, "MB"),
                DisplayUnit.GB => (value / BytesToGB, "GB"),
                DisplayUnit.TB => (value / BytesToTB, "TB"),
                DisplayUnit.Percent => (value, "%"),
                DisplayUnit.Celsius => (value, "°C"),
                _ => (value, "")
            };
        }
    }
}
