using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models.MonitorProviderSettings
{
    public partial class NetworkTrafficSettings : ObservableObject
    {
        [ObservableProperty]
        private string _selectedInterfaceId = string.Empty;

        [ObservableProperty]
        private ObservableCollection<NetworkInterfaceInfo> _availableInterfaces = [];

        [ObservableProperty]
        private bool _showSeparateUpDown = true;
    }

    public class NetworkInterfaceInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public string DisplayText => $"{Name} ({Description})";
    }
}