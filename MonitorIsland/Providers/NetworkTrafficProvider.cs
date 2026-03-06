using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using System.Net.NetworkInformation;
using MonitorIsland.Models.MonitorProviderSettings;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.networktraffic",
        "网络流量",
        "监控网络上下行流量",
        [DisplayUnit.MBps, DisplayUnit.KBps, DisplayUnit.Mbps, DisplayUnit.Kbps])]
    public class NetworkTrafficProvider : MonitorProviderBase<NetworkTrafficSettings>
    {
        public override string DefaultPrefix => "网络: ";

        private NetworkInterface? _networkInterface;
        private long _lastBytesSent;
        private long _lastBytesReceived;
        private DateTime _lastTime;

        public override string? GetData()
        {
            var interfaceId = Settings.SelectedInterfaceId;
            if (_networkInterface == null || _networkInterface.Id != interfaceId)
            {
                InitializeInterface(interfaceId);
            }

            if (_networkInterface == null || !IsInterfaceValid(_networkInterface))
            {
                return null;
            }

            var stats = _networkInterface.GetIPv4Statistics();
            var currentBytesSent = stats.BytesSent;
            var currentBytesReceived = stats.BytesReceived;
            var currentTime = DateTime.Now;

            var timeSpan = (currentTime - _lastTime).TotalSeconds;
            if (timeSpan > 0 && _lastTime != default)
            {
                var sentSpeedBps = (currentBytesSent - _lastBytesSent) / timeSpan;
                var receivedSpeedBps = (currentBytesReceived - _lastBytesReceived) / timeSpan;

                _lastBytesSent = currentBytesSent;
                _lastBytesReceived = currentBytesReceived;
                _lastTime = currentTime;

                var unit = SelectedUnit ?? DisplayUnit.MBps;
                var (sentValue, receivedValue) = ConvertSpeed(sentSpeedBps, receivedSpeedBps, unit);

                return $"↑{sentValue} ↓{receivedValue}";
            }

            _lastBytesSent = currentBytesSent;
            _lastBytesReceived = currentBytesReceived;
            _lastTime = currentTime;

            return "↑0 ↓0";
        }

        private (string sent, string received) ConvertSpeed(double bytesSentPerSec, double bytesReceivedPerSec, DisplayUnit unit)
        {
            var sent = bytesSentPerSec * 8;
            var received = bytesReceivedPerSec * 8;

            double sentValue, receivedValue;

            switch (unit)
            {
                case DisplayUnit.Kbps:
                    sentValue = sent / 1000;
                    receivedValue = received / 1000;
                    return ($"{sentValue:F1}", $"{receivedValue:F1}");
                    
                case DisplayUnit.Mbps:
                    sentValue = sent / 1000 / 1000;
                    receivedValue = received / 1000 / 1000;
                    return ($"{sentValue:F1}", $"{receivedValue:F1}");
                    
                case DisplayUnit.KBps:
                    sentValue = bytesSentPerSec / 1024;
                    receivedValue = bytesReceivedPerSec / 1024;
                    return ($"{sentValue:F2}", $"{receivedValue:F2}");
                    
                case DisplayUnit.MBps:
                default:
                    sentValue = bytesSentPerSec / 1024 / 1024;
                    receivedValue = bytesReceivedPerSec / 1024 / 1024;
                    return ($"{sentValue:F2}", $"{receivedValue:F2}");
            }
        }

        private bool IsInterfaceValid(NetworkInterface ni)
        {
            return ni.NetworkInterfaceType != NetworkInterfaceType.Loopback 
                && ni.OperationalStatus == OperationalStatus.Up;
        }

        private void InitializeInterface(string? interfaceId)
        {
            if (string.IsNullOrWhiteSpace(interfaceId))
            {
                _networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => IsInterfaceValid(ni));
            }
            else
            {
                _networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.Id == interfaceId);
            }

            if (_networkInterface != null)
            {
                var stats = _networkInterface.GetIPv4Statistics();
                _lastBytesSent = stats.BytesSent;
                _lastBytesReceived = stats.BytesReceived;
                _lastTime = DateTime.Now;
            }
        }
    }
}
