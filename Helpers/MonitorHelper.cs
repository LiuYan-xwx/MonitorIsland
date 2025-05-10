using System.Diagnostics;
using LibreHardwareMonitor.Hardware;

namespace MonitorIsland.Helpers
{
    public class MonitorHelper : IDisposable
    {
        private readonly Lazy<PerformanceCounter> _memoryCounter = new(() => new PerformanceCounter("Memory", "Available MBytes"));
        private readonly Lazy<PerformanceCounter> _cpuCounter = new(() =>
        {
            var counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            counter.NextValue(); // 预热
            return counter;
        });
        private readonly Lazy<Computer> _computer = new(() =>
        {
            var computer = new Computer
            {
                IsCpuEnabled = true // 启用 CPU 监控
            };
            computer.Open();
            return computer;
        });

        private bool _disposed;

        public float GetMemoryUsage()
        {
            var totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
            var availableMemory = _memoryCounter.Value.NextValue();
            return totalMemory - availableMemory;
        }

        public float GetCpuUsage()
        {
            return _cpuCounter.Value.NextValue();
        }

        public float GetCpuTemperature()
        {
            float temperature = -1;

            try
            {
                foreach (var hardware in _computer.Value.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Cpu)
                    {
                        hardware.Update();
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

        public void Dispose()
        {
            if (_disposed) return;

            if (_memoryCounter.IsValueCreated)
            {
                _memoryCounter.Value.Dispose();
            }

            if (_cpuCounter.IsValueCreated)
            {
                _cpuCounter.Value.Dispose();
            }

            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
            }

            _disposed = true;
        }
    }
}
