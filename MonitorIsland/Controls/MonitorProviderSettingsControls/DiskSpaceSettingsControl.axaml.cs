using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions.Controls;
using MonitorIsland.Models.MonitorProviderSettings;

namespace MonitorIsland.Controls.MonitorProviderSettingsControls;

public partial class DiskSpaceSettingsControl : MonitorProviderControlBase<DiskSpaceSettings>
{
    private readonly ILogger<DiskSpaceSettingsControl> Logger;

    public DiskSpaceSettingsControl(ILogger<DiskSpaceSettingsControl> logger)
    {
        InitializeComponent();
        Logger = logger;
    }

    private void MonitorProviderControlBase_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        LoadAvailableDrives();
    }

    private void LoadAvailableDrives()
    {
        if (Settings.AvailableDriveNames.Count > 0)
        {
            return;
        }
        Settings.AvailableDriveNames.Clear();
        try
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    Settings.AvailableDriveNames.Add(drive.Name);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "加载可用驱动器时发生错误");
        }
    }
}