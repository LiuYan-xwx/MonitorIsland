using ClassIsland.Core.Helpers.UI;
using FluentAvalonia.UI.Controls;
using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Models;

namespace MonitorIsland.Services
{
    /// <summary>
    /// 硬件监控服务，管理 LibreHardwareMonitor 实例
    /// </summary>
    public sealed class HardwareMonitorService : IDisposable
    {
        private readonly ILogger<HardwareMonitorService> _logger;
        private readonly object _lock = new();
        private Computer? _computer;
        private volatile bool _isReady;
        private readonly Dictionary<string, ISensor> _sensorCache = [];
        private readonly Dictionary<string, SensorInfo> _sensorInfos = [];
        private bool _disposed;

        private readonly Task _readyTask;
        public bool IsReady { get => _isReady; private set => _isReady = value; }

        public Task ReadyTask => _readyTask;

        public HardwareMonitorService(ILogger<HardwareMonitorService> logger)
        {
            _logger = logger;

            _readyTask = InitializeAsync();
        }

        /// <summary>
        /// 后台初始化硬件监控（不阻塞启动）
        /// </summary>
        private async Task InitializeAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    _computer = new Computer
                    {
                        IsCpuEnabled = true,
                        IsGpuEnabled = true,
                        IsMotherboardEnabled = true,
                        IsStorageEnabled = true,
                        IsMemoryEnabled = true,
                        IsNetworkEnabled = true,
                        IsBatteryEnabled = true
                    };
                    _computer.Open();

                    CacheSensorsInternal();
                });

                IsReady = true;
                _logger.LogInformation("LibreHardwareMonitor 已初始化");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化 LibreHardwareMonitor 失败");
            }
        }

        private void CacheSensorsInternal()
        {
            _sensorCache.Clear();
            _sensorInfos.Clear();

            if (!IsReady) return;

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();

                foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                {
                    _sensorCache[sensor.Identifier.ToString()] = sensor;
                    _sensorInfos[sensor.Identifier.ToString()] = new SensorInfo
                    {
                        Identifier = sensor.Identifier.ToString(),
                        Name = sensor.Name,
                        HardwareName = hardware.Name,
                        SensorType = sensor.SensorType
                    };
                }
            }
        }

        /// <summary>
        /// 获取按硬件分组的传感器树，用于 TreeView 展示
        /// </summary>
        /// <param name="sensorTypes">要包含的传感器类型，默认仅 Temperature</param>
        public List<SensorTreeNode> GetSensorTree(HashSet<SensorType>? sensorTypes = null)
        {
            if (!IsReady) return [];

            var types = sensorTypes ?? [SensorType.Temperature];
            var tree = new List<SensorTreeNode>();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();

                var sensors = hardware.Sensors
                    .Where(s => types.Contains(s.SensorType))
                    .OrderBy(s => s.SensorType)
                    .ThenBy(s => s.Name)
                    .ToList();

                if (sensors.Count == 0)
                    continue;

                var hwNode = new SensorTreeNode
                {
                    DisplayName = hardware.Name,
                    IsSensor = false,
                    IsExpanded = false,
                    HardwareIconGlyph = GetHardwareIconGlyph(hardware.HardwareType)
                };

                foreach (var sensor in sensors)
                {
                    hwNode.Children.Add(new SensorTreeNode
                    {
                        DisplayName = sensor.Name,
                        IsSensor = true,
                        Sensor = new SensorInfo
                        {
                            Identifier = sensor.Identifier.ToString(),
                            Name = sensor.Name,
                            HardwareName = hardware.Name,
                            SensorType = sensor.SensorType
                        }
                    });
                }

                tree.Add(hwNode);
            }

            return tree;
        }

        /// <summary>
        /// 根据硬件类型返回对应的 Fluent 图标 Glyph
        /// </summary>
        private static IconSource GetHardwareIconGlyph(HardwareType hardwareType)
        {
            return hardwareType switch
            {
                HardwareType.Cpu => IconExpressionHelper.Parse("lucide(\uE0AD)"),
                HardwareType.GpuNvidia => IconExpressionHelper.Parse("lucide(\uE66F)"),
                HardwareType.GpuAmd => IconExpressionHelper.Parse("lucide(\uE66F)"),
                HardwareType.GpuIntel => IconExpressionHelper.Parse("lucide(\uE66F)"),
                HardwareType.Motherboard => IconExpressionHelper.Parse("lucide(\uE408)"),
                HardwareType.Storage => IconExpressionHelper.Parse("lucide(\uE0F1)"),
                HardwareType.Memory => IconExpressionHelper.Parse("lucide(\uE44A)"),
                HardwareType.Network => IconExpressionHelper.Parse("lucide(\uE129)"),
                HardwareType.Battery => IconExpressionHelper.Parse("lucide(\uE059)"),
                HardwareType.SuperIO => IconExpressionHelper.Parse("lucide(\uE4ED)"),
                HardwareType.Cooler => IconExpressionHelper.Parse("lucide(\uE37D)"),
                _ => IconExpressionHelper.Parse("lucide(\uE565)")
            };
        }

        /// <summary>
        /// 获取指定传感器的当前值
        /// </summary>
        /// <param name="identifier">传感器标识符</param>
        /// <returns>传感器值，如果传感器不存在或无值则返回 null</returns>
        public float? GetSensorValue(string identifier)
        {
            if (!IsReady) return null;

            // 尝试从缓存获取
            if (_sensorCache.TryGetValue(identifier, out var cachedSensor))
            {
                cachedSensor.Hardware.Update();
                if (cachedSensor.Value.HasValue)
                    return cachedSensor.Value.Value;
            }

            CacheSensorsInternal();

            // 重新从缓存获取
            if (_sensorCache.TryGetValue(identifier, out var cachedSensor2))
            {
                cachedSensor2.Hardware.Update();
                if (cachedSensor2.Value.HasValue)
                    return cachedSensor2.Value.Value;
            }

            _logger.LogWarning("传感器 {Identifier} 未找到任何匹配的传感器", identifier);
            return null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            IsReady = false;

            _sensorCache.Clear();
            lock (_lock)
            {
                _computer?.Close();
                _computer = null;
            }
            _logger.LogInformation("LibreHardwareMonitor 已关闭");
        }
    }
}
