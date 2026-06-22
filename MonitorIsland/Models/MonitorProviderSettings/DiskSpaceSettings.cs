using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models.MonitorProviderSettings
{
    public partial class DiskSpaceSettings : ObservableObject
    {
        [ObservableProperty]
        private string? _driveName;

        [ObservableProperty]
        private ObservableCollection<string> _availableDriveNames = [];
    }
}
