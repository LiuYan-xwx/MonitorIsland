using Avalonia.Controls;
using MonitorIsland.Models;
using System.Collections.ObjectModel;

namespace MonitorIsland.Controls.MonitorProviderSettingsControls;

public partial class SensorTreePanel : UserControl
{
    public ObservableCollection<SensorTreeNode> SensorTreeNodes { get; } = [];

    /// <summary>
    /// 传感器被选中时触发
    /// </summary>
    public event EventHandler<SensorInfo?>? SensorSelected;

    public SensorTreePanel()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// 设置 TreeView 的数据源并恢复之前的选中状态
    /// </summary>
    /// <param name="nodes">传感器树节点列表</param>
    /// <param name="selectedSensor">当前已选中的传感器（用于恢复选中），可为 null</param>
    public void SetData(IEnumerable<SensorTreeNode> nodes, SensorInfo? selectedSensor)
    {
        SensorTreeNodes.Clear();
        foreach (var node in nodes)
            SensorTreeNodes.Add(node);

        RestoreSelection(selectedSensor);
    }

    /// <summary>
    /// 恢复之前的传感器选中状态，并展开父节点链
    /// </summary>
    private void RestoreSelection(SensorInfo? selectedSensor)
    {
        if (selectedSensor == null) return;

        foreach (var hwNode in SensorTreeNodes)
        {
            foreach (var sensorNode in hwNode.Children)
            {
                if (sensorNode.IsSensor
                    && sensorNode.Sensor?.Identifier == selectedSensor.Identifier)
                {
                    hwNode.IsExpanded = true;
                    SensorTreeView.SelectedItem = sensorNode;
                    return;
                }
            }
        }
    }

    private void SensorTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var node = SensorTreeView.SelectedItem as SensorTreeNode;
        if (node?.IsSensor == true && node.Sensor != null)
        {
            SensorSelected?.Invoke(this, node.Sensor);
        }
    }
}
