using Avalonia.Data.Converters;
using System.Globalization;

namespace MonitorIsland.Converters
{
    /// <summary>
    /// 网络流量提供方时隐藏某些设置
    /// </summary>
    public class NetworkTrafficVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 如果是网络流量，返回 false（隐藏）；其他返回 true（显示）
            if (value is string providerId)
            {
                return providerId != "monitorisland.networktraffic";
            }
            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}