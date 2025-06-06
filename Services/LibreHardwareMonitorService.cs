using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorIsland.Services
{
    public class LibreHardwareMonitorService : IDisposable
    {
        private readonly ILogger<LibreHardwareMonitorService> _logger;
        private readonly Lazy<Computer> _computer;
        private ISensor? _cpuTempSensor;
        private int _disposed;

        public LibreHardwareMonitorService(ILogger<LibreHardwareMonitorService> logger)
        {
            _logger = logger;
            _computer = new Lazy<Computer>(() =>
            {
                var computer = new Computer
                {
                    IsCpuEnabled = true
                };
                computer.Open();
                TryFindCpuTempSensor(computer);
                return computer;
            });
        }

        private void TryFindCpuTempSensor(Computer computer)
        {
            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            if (cpu != null)
            {
                // 优先找 "CPU Package"
                _cpuTempSensor = cpu.Sensors
                    .FirstOrDefault(s => s.SensorType == SensorType.Temperature &&
                                         s.Name.Equals("CPU Package", StringComparison.OrdinalIgnoreCase) &&
                                         s.Value.HasValue);

                if (_cpuTempSensor != null)
                {
                    _logger.LogDebug("已找到 CPU 温度传感器: {SensorName}", _cpuTempSensor.Name);
                }
                else
                {
                    // 没有 "CPU Package" 就找 "Core Average"
                    _cpuTempSensor = cpu.Sensors
                        .FirstOrDefault(s => s.SensorType == SensorType.Temperature &&
                                             s.Name.Equals("Core Average", StringComparison.OrdinalIgnoreCase) &&
                                             s.Value.HasValue);

                    if (_cpuTempSensor != null)
                    {
                        _logger.LogDebug("已找到 CPU 温度传感器: {SensorName}", _cpuTempSensor.Name);
                    }
                    else
                    {
                        _logger.LogError("未找到 CPU Package 或 Core Average 传感器");
                    }
                }
            }
            else
            {
                _logger.LogError("未找到 CPU 硬件");
            }
        }

        public float GetCpuTemperature()
        {
            try
            {
                var computer = _computer.Value;

                // 检查传感器是否失效
                if (_cpuTempSensor == null || _cpuTempSensor.Hardware == null)
                {
                    _logger.LogDebug("重新查找 CPU 传感器");
                    TryFindCpuTempSensor(computer);

                    if (_cpuTempSensor == null || _cpuTempSensor.Hardware == null)
                    {
                        _logger.LogError("未找到 CPU 温度传感器");
                        return -1;
                    }
                }

                _cpuTempSensor.Hardware.Update();

                if (_cpuTempSensor.Value is float temp)
                {
                    return temp;
                }
                else
                {
                    _logger.LogError("CPU 温度传感器值为 null");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取 CPU 温度失败");
                return -1;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
                _logger.LogDebug("释放硬件监控组件资源");
            }
        }
    }
}
