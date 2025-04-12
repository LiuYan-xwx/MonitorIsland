using ClassIsland.Core.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorIsland.Models.ComponentSettings
{
    public class MonitorComponentSettings : ObservableRecipient
    {
        private int _monitorType;
        private float _memoryUsage;
        private float _cpuUsage;
        private float _cpuTemperature;
        private int _refreshInterval=1000;

        /// <summary>
        /// 0 - 内存使用量<br/>
        /// 1 - CPU 利用率<br/>
        /// 2 - CPU 温度<br/>
        /// </summary>
        public int MonitorType
        {
            get => _monitorType;
            set
            {
                if (value == _monitorType) return;
                _monitorType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前内存使用量（MB）
        /// </summary>
        public float MemoryUsage
        {
            get => _memoryUsage;
            set
            {
                if (Math.Abs(value - _memoryUsage) < 0.01f) return;
                _memoryUsage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前 CPU 利用率（%）
        /// </summary>
        public float CpuUsage
        {
            get => _cpuUsage;
            set
            {
                if (Math.Abs(value - _cpuUsage) < 0.01f) return;
                _cpuUsage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前 CPU 温度（摄氏度）
        /// </summary>
        public float CpuTemperature
        {
            get => _cpuTemperature;
            set
            {
                if (Math.Abs(value - _cpuTemperature) < 0.01f) return;
                _cpuTemperature = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 刷新间隔（毫秒）
        /// </summary>
        public int RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                if (value == _refreshInterval) return;
                _refreshInterval = value;
                OnPropertyChanged();
            }
        }
    }
}
