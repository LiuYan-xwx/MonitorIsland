using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace MonitorIsland.Models
{
    /// <summary>
    /// 代表一个监控项。
    /// </summary>
    public partial class MonitorProvider : ObservableObject
    {
        /// <summary>
        /// 监控项 ID。
        /// </summary>
        [ObservableProperty]
        private string _id = string.Empty;

        /// <summary>
        /// 监控项名称
        /// </summary>
        [ObservableProperty]
        private string _name = string.Empty;

        /// <summary>
        /// 监控项设置。
        /// </summary>
        [ObservableProperty]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        private object? _settings;

        /// <summary>
        /// 深拷贝基础字段
        /// </summary>
        public MonitorProvider CopyWithoutSettings()
        {
            return new MonitorProvider
            {
                Id = Id,
                Name = Name,
                Settings = null
            };
        }
    }
}
