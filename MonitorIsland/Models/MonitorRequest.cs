using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorIsland.Models;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 取数请求上下文（调用 GetData 时传入）。
    /// </summary>
    public readonly record struct MonitorRequest
    {
        /// <summary>
        /// 当前选择的显示单位
        /// </summary>
        public DisplayUnit? SelectedUnit { get; init; }

        public MonitorRequest(DisplayUnit? selectedUnit)
        {
            SelectedUnit = selectedUnit;
        }

        public static MonitorRequest FromSelectedUnit(DisplayUnit? selectedUnit) => new(selectedUnit);
    }
}
