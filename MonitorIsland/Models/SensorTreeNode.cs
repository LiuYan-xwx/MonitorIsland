using Avalonia.Media;
using ClassIsland.Core.Helpers.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using MonitorIsland.Abstractions.Models;
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
        /// 硬件类型（硬件节点使用）
        /// </summary>
        public HardwareKind HardwareKind { get; set; }

        /// <summary>
        /// 节点显示的图标 Glyph（硬件节点返回对应硬件图标，传感器节点返回温度计图标）
        /// </summary>
        public IconSource IconGlyph => IsSensor
            ? IconExpressionHelper.Parse("fluent(\uF1B4)")
            : HardwareKind switch
            {
                HardwareKind.Cpu => IconExpressionHelper.Parse("lucide(\uE0AD)"),
                HardwareKind.Gpu => IconExpressionHelper.Parse("lucide(\uE66F)"),
                HardwareKind.Motherboard => IconExpressionHelper.Parse("lucide(\uE408)"),
                HardwareKind.Storage => IconExpressionHelper.Parse("lucide(\uE0F1)"),
                HardwareKind.Memory => IconExpressionHelper.Parse("lucide(\uE44A)"),
                HardwareKind.Network => IconExpressionHelper.Parse("lucide(\uE129)"),
                HardwareKind.Battery => IconExpressionHelper.Parse("lucide(\uE059)"),
                HardwareKind.SuperIo => IconExpressionHelper.Parse("lucide(\uE4ED)"),
                HardwareKind.Cooler => IconExpressionHelper.Parse("lucide(\uE37D)"),
                _ => IconExpressionHelper.Parse("lucide(\uE565)")
            };

        /// <summary>
        /// 传感器类型显示文本（仅传感器叶子节点有值）
        /// </summary>
        public string? SensorTypeLabel => Sensor?.SensorType switch
        {
            SensorKind.Temperature => "温度",
            SensorKind.Load => "负载",
            SensorKind.Clock => "频率",
            SensorKind.Fan => "风扇",
            SensorKind.Flow => "流量",
            SensorKind.Control => "控制",
            SensorKind.Level => "液位",
            SensorKind.Power => "功耗",
            SensorKind.Data => "数据",
            SensorKind.Voltage => "电压",
            SensorKind.Current => "电流",
            SensorKind.Factor => "系数",
            SensorKind.Frequency => "频率",
            SensorKind.Energy => "能量",
            SensorKind.Noise => "噪声",
            SensorKind.Humidity => "湿度",
            SensorKind.Throughput => "吞吐",
            SensorKind.TimeSpan => "时间",
            SensorKind.Timing => "时序",
            SensorKind.SmallData => "小数据",
            SensorKind.Conductivity => "电导率",
            _ => null
        };

        /// <summary>
        /// 传感器类型对应的颜色画刷（仅传感器叶子节点有值）
        /// </summary>
        public IBrush? SensorTypeColorBrush => Sensor?.SensorType switch
        {
            SensorKind.Temperature => BrushFor("#FF7043"),
            SensorKind.Load => BrushFor("#42A5F5"),
            SensorKind.Clock => BrushFor("#AB47BC"),
            SensorKind.Fan => BrushFor("#26C6DA"),
            SensorKind.Power => BrushFor("#FFCA28"),
            SensorKind.Voltage => BrushFor("#66BB6A"),
            SensorKind.Current => BrushFor("#EF5350"),
            SensorKind.Frequency => BrushFor("#AB47BC"),
            SensorKind.Energy => BrushFor("#FFCA28"),
            SensorKind.Flow => BrushFor("#26C6DA"),
            SensorKind.Control => BrushFor("#42A5F5"),
            SensorKind.Level => BrushFor("#8D6E63"),
            SensorKind.Noise => BrushFor("#BDBDBD"),
            SensorKind.Humidity => BrushFor("#42A5F5"),
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
