using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.SystemInformation;

namespace MonitorIsland.Helpers;

[SupportedOSPlatform("windows")]
internal sealed class WindowsSystemMetrics
{
    private readonly Lazy<PerformanceCounter> _availableMemoryCounter = new(() =>
    {
        var counter = new PerformanceCounter("Memory", "Available Bytes");
        counter.NextValue();
        return counter;
    });

    private readonly Lazy<PerformanceCounter> _cpuCounter = new(() =>
    {
        var counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        counter.NextValue();
        return counter;
    });

    public double? GetCpuUsage()
    {
        return ReadCounter(_cpuCounter.Value);
    }

    public ulong? GetTotalMemory()
    {
        var memoryInfo = new MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>()
        };

        return PInvoke.GlobalMemoryStatusEx(ref memoryInfo)
            ? memoryInfo.ullTotalPhys
            : null;
    }

    public double? GetAvailableMemory()
    {
        return ReadCounter(_availableMemoryCounter.Value);
    }

    private static double? ReadCounter(PerformanceCounter? counter)
    {
        try
        {
            return counter?.NextValue();
        }
        catch
        {
            return null;
        }
    }
}
