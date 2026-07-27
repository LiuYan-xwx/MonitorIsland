using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
        var memoryInfo = new MemoryStatusEx
        {
            Length = (uint)Marshal.SizeOf<MemoryStatusEx>()
        };

        return GlobalMemoryStatusEx(ref memoryInfo)
            ? memoryInfo.TotalPhysical
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

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryStatusEx
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhysical;
        public ulong AvailablePhysical;
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(
        ref MemoryStatusEx memoryStatus);
}
