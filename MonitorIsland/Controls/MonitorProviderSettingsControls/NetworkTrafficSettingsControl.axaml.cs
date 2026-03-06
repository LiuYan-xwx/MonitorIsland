using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Models.MonitorProviderSettings;
using System.Net.NetworkInformation;

namespace MonitorIsland.Controls.MonitorProviderSettingsControls;

public partial class NetworkTrafficSettingsControl : MonitorProviderControlBase<NetworkTrafficSettings>
{
    private readonly ILogger<NetworkTrafficSettingsControl> _logger;

    public NetworkTrafficSettingsControl(ILogger<NetworkTrafficSettingsControl> logger)
    {
        InitializeComponent();
        _logger = logger;
    }

    private void NetworkTrafficSettingsControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        LoadAvailableInterfaces();
    }

    private void LoadAvailableInterfaces()
    {
        if (Settings.AvailableInterfaces.Count > 0)
        {
            return;
        }

        Settings.AvailableInterfaces.Clear();
        
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderBy(ni => ni.OperationalStatus == OperationalStatus.Up ? 0 : 1);

            foreach (var ni in interfaces)
            {
                Settings.AvailableInterfaces.Add(new NetworkInterfaceInfo
                {
                    Id = ni.Id,
                    Name = ni.Name,
                    Description = ni.Description
                });
            }

            // 如果没有选中项，默认选第一个可用的
            if (string.IsNullOrWhiteSpace(Settings.SelectedInterfaceId))
            {
                var firstUp = Settings.AvailableInterfaces.FirstOrDefault();
                if (firstUp != null)
                {
                    Settings.SelectedInterfaceId = firstUp.Id;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载网络接口时发生错误");
        }
    }
}