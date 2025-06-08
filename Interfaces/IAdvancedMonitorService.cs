using MonitorIsland.Models;
using System;
using System.Collections.Generic;

namespace MonitorIsland.Interfaces
{
    public interface IAdvancedMonitorService : IDisposable
    {
        List<HardwareInfo> GetAllAvailableHardware();
        string GetSensorValue(string hardwareId);
    }
}