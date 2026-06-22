using Avalonia.Media;
using ClassIsland.Core.Helpers.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using LibreHardwareMonitor.Hardware;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 传感器树节点，用于 TreeView 展示
    /// </summary>
    public partial class SensorTreeNode : ObservableObject
    {
        /// <summary>
        /// 节点显示名称
        /// </summary>
        [ObservableProperty]
        private string _displayName = string.Empty;

        /// <summary>
        /// 是否为传感器叶子节点
        /// </summary>
        [ObservableProperty]
        private bool _isSensor;

        /// <summary>
        /// 关联的传感器信息（仅叶子节点有值）
        /// </summary>
        public SensorInfo? Sensor { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public ObservableCollection<SensorTreeNode> Children { get; set; } = [];

        /// <summary>
        /// 是否为展开状态
        /// </summary>
        [ObservableProperty]
        private bool _isExpanded = false;

        /// <summary>
        /// Fluent 图标 Glyph（硬件节点使用，按 HardwareType 分配）
        /// </summary>
        public IconSource HardwareIconGlyph { get; set; } = IconExpressionHelper.Parse("lucide(\uE0F1)");

        /// <summary>
        /// 节点显示的图标 Glyph（硬件节点返回对应硬件图标，传感器节点返回温度计图标）
        /// </summary>
        public IconSource IconGlyph => IsSensor ? IconExpressionHelper.Parse("fluent(\uF1B4)") : HardwareIconGlyph;

        /// <summary>
        /// 传感器类型显示文本（仅传感器叶子节点有值）
        /// </summary>
        public string? SensorTypeLabel => Sensor?.SensorType switch
        {
            SensorType.Temperature => "温度",
            SensorType.Load => "负载",
            SensorType.Clock => "频率",
            SensorType.Fan => "风扇",
            SensorType.Flow => "流量",
            SensorType.Control => "控制",
            SensorType.Level => "液位",
            SensorType.Power => "功耗",
            SensorType.Data => "数据",
            SensorType.Voltage => "电压",
            SensorType.Current => "电流",
            SensorType.Factor => "系数",
            SensorType.Frequency => "频率",
            SensorType.Energy => "能量",
            SensorType.Noise => "噪声",
            SensorType.Humidity => "湿度",
            SensorType.Throughput => "吞吐",
            SensorType.TimeSpan => "时间",
            SensorType.SmallData => "小数据",
            _ => null
        };

        /// <summary>
        /// 传感器类型对应的颜色画刷（仅传感器叶子节点有值）
        /// </summary>
        public IBrush? SensorTypeColorBrush => Sensor?.SensorType switch
        {
            SensorType.Temperature => BrushFor("#FF7043"),
            SensorType.Load => BrushFor("#42A5F5"),
            SensorType.Clock => BrushFor("#AB47BC"),
            SensorType.Fan => BrushFor("#26C6DA"),
            SensorType.Power => BrushFor("#FFCA28"),
            SensorType.Voltage => BrushFor("#66BB6A"),
            SensorType.Current => BrushFor("#EF5350"),
            SensorType.Frequency => BrushFor("#AB47BC"),
            SensorType.Energy => BrushFor("#FFCA28"),
            SensorType.Flow => BrushFor("#26C6DA"),
            SensorType.Control => BrushFor("#42A5F5"),
            SensorType.Level => BrushFor("#8D6E63"),
            SensorType.Noise => BrushFor("#BDBDBD"),
            SensorType.Humidity => BrushFor("#42A5F5"),
            _ => BrushFor("#BDBDBD")
        };

        private static readonly Dictionary<string, IBrush> _brushCache = [];

        private static IBrush BrushFor(string hex)
        {
            if (!_brushCache.TryGetValue(hex, out var brush))
            {
                brush = SolidColorBrush.Parse(hex);
                _brushCache[hex] = brush;
            }
            return brush;
        }
    }
}
