using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorIsland.Interfaces
{
    public interface IMonitorService : IDisposable
    {
        string GetFormattedMonitorValue(int monitorType);
        float GetMemoryUsage();
        float GetCpuUsage();
        float GetCpuTemperature();
    }
}
