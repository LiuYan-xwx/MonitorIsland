using MonitorIsland.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MonitorIsland.Converters
{
    public class MonitorOptionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MonitorOption option)
            {
                return option switch
                {
                    MonitorOption.MemoryUsage => "内存使用量",
                    MonitorOption.MemoryUsageRate => "内存使用率",
                    MonitorOption.CpuUsage => "CPU 利用率",
                    MonitorOption.CpuTemperature => "CPU 温度",
                    MonitorOption.DiskSpace => "磁盘空间",
                    _ => value.ToString() ?? string.Empty // 作为备选方案，显示枚举的原始名称
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ComboBox 的 SelectedItem 直接绑定到枚举类型时，
            // WPF 通常能处理好从选中项到枚举值的转换，这个方法可能不需要复杂实现。
            // 如果遇到问题，可能需要根据 ComboBox 的具体绑定方式来调整。
            // 对于当前 ItemSource 是枚举值，SelectedItem 也是枚举值的情况，此方法一般不会被调用或依赖。
            throw new NotSupportedException("ConvertBack is not supported for MonitorOptionToStringConverter.");
        }
    }
}
