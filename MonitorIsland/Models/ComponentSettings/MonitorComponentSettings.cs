using CommunityToolkit.Mvvm.ComponentModel;
using MonitorIsland.Abstractions;
using System.Text.Json.Serialization;

namespace MonitorIsland.Models.ComponentSettings
{
    public partial class MonitorComponentSettings : ObservableObject
    {
        [ObservableProperty]
        [property: JsonIgnore]
        private string _displayData = string.Empty;

        [ObservableProperty]
        private string _displayPrefix = string.Empty;

        [ObservableProperty]
        private bool _showUnit = true;

        [ObservableProperty]
        private MonitorProvider? _selectedProvider = null;

        [ObservableProperty]
        [property: JsonIgnore]
        private MonitorProviderBase? _selectedProviderBase = null;

        [ObservableProperty]
        [property: JsonIgnore]
        private string? _selectedProviderId;

        [ObservableProperty]
        private DisplayUnit? _selectedUnit;

        [ObservableProperty]
        private List<DisplayUnit> _availableUnits = [];

        [ObservableProperty]
        private int _decimalPlaces = 2;

        [ObservableProperty]
        private bool _showProviderSettingsControl = false;

        [ObservableProperty]
        private int _refreshInterval = 1000;

    }
}
