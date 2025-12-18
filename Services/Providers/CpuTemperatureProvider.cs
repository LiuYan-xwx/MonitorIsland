using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;

namespace MonitorIsland.Services.Providers
{
    /// <summary>
    /// CPU�¶ȼ���ṩ��
    /// </summary>
    public class CpuTemperatureProvider : IMonitorProvider
    {
        private readonly ILogger<CpuTemperatureProvider> _logger;
        private readonly Dictionary<string, ISensor> _temperatureSensors = [];
        private readonly Lazy<Computer> _computer;
        private int _disposed;

        public IReadOnlyList<MonitorOption> SupportedTypes { get; } = new[]
        {
            MonitorOption.CpuTemperature
        };

        public bool IsAvailable { get; private set; }

        public CpuTemperatureProvider(ILogger<CpuTemperatureProvider> logger)
        {
            _logger = logger;
            _computer = new Lazy<Computer>(() =>
            {
                _logger.LogDebug("��ʼ��Ӳ��������");
                var computer = new Computer
                {
                    IsCpuEnabled = true
                };
                computer.Open();
                return computer;
            });
            IsAvailable = true;
        }

        public void Initialize()
        {
            try
            {
                // Ԥ���ش������б�
                LoadAvailableSensors();
                _logger.LogInformation("CPU�¶ȼ���ṩ����ʼ���ɹ����ҵ� {Count} ��������", _temperatureSensors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CPU�¶ȼ���ṩ����ʼ��ʧ��");
                IsAvailable = false;
            }
        }

        public float? GetValue(MonitorRequest request)
        {
            if (!IsAvailable || request.MonitorType != MonitorOption.CpuTemperature)
                return null;

            try
            {
                if (string.IsNullOrEmpty(request.CpuTemperatureSensorId))
                {
                    _logger.LogWarning("δָ�� CPU �¶ȴ�����ID");
                    return null;
                }

                if (!_temperatureSensors.TryGetValue(request.CpuTemperatureSensorId, out var sensor))
                {
                    _logger.LogWarning("δ�ҵ�ָ���� CPU �¶ȴ�����ID: {SensorId}", request.CpuTemperatureSensorId);
                    return null;
                }

                sensor.Hardware.Update();

                if (sensor.Value.HasValue)
                    return sensor.Value.Value;

                _logger.LogWarning("������ {SensorId} û�п��õ��¶�ֵ", request.CpuTemperatureSensorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "��ȡ CPU �¶�ʧ��");
            }

            return null;
        }

        public List<CpuTemperatureSensorInfo> GetAvailableSensors()
        {
            LoadAvailableSensors();
            return _temperatureSensors.Select(sensorPair => new CpuTemperatureSensorInfo
            {
                Id = sensorPair.Key,
                Name = sensorPair.Value.Name,
                HardwareName = sensorPair.Value.Hardware.Name
            }).ToList();
        }

        private void LoadAvailableSensors()
        {
            _temperatureSensors.Clear();

            try
            {
                var computer = _computer.Value;

                foreach (var hardware in computer.Hardware.Where(hw => hw.HardwareType == HardwareType.Cpu))
                {
                    hardware.Update();

                    foreach (var sensor in hardware.Sensors.Where(sen => sen.SensorType == SensorType.Temperature))
                    {
                        var sensorId = $"{hardware.Identifier}_{sensor.Identifier}";
                        _temperatureSensors[sensorId] = sensor;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "���ؿ��� CPU �¶ȴ�����ʧ��");
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            if (_computer.IsValueCreated)
            {
                _computer.Value.Close();
                _temperatureSensors.Clear();
                _logger.LogDebug("�ͷ�Ӳ����������Դ");
            }
        }
    }
}