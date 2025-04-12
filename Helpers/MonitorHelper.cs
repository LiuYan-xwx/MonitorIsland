using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorIsland.Helpers
{
    public class MonitorHelper
    {
        private readonly PerformanceCounter _memoryCounter;
        private readonly PerformanceCounter _cpuCounter;
        private readonly Computer _computer;

        public MonitorHelper()
        {
            // 初始化内存计数器
            _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            // 初始化 CPU 计数器
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // 预热 CPU 计数器，丢弃第一次调用的值
            _cpuCounter.NextValue();

            // 初始化 LibreHardwareMonitor
            _computer = new Computer
            {
                IsCpuEnabled = true // 启用 CPU 监控
            };
            _computer.Open();
        }

        /// <summary>
        /// 获取当前系统的内存使用量（MB）
        /// </summary>
        /// <returns>已使用内存（MB）</returns>
        public float GetMemoryUsage()
        {
            var totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
            var availableMemory = _memoryCounter.NextValue();
            return totalMemory - availableMemory;
        }

        /// <summary>
        /// 获取当前系统的 CPU 使用率（%）
        /// </summary>
        /// <returns>CPU 使用率（%）</returns>
        public float GetCpuUsage()
        {
            return _cpuCounter.NextValue();
        }

        /// <summary>
        /// 获取当前系统的 CPU 温度（摄氏度）
        /// </summary>
        /// <returns>CPU 温度（摄氏度）</returns>
        public float GetCpuTemperature()
        {
            float temperature = -1;

            try
            {
                foreach (var hardware in _computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        hardware.Update(); // 更新硬件数据
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                temperature = sensor.Value.GetValueOrDefault();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取 CPU 温度失败: {ex.Message}");
            }

            return temperature;
        }
    }
}
