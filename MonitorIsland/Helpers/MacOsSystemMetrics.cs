using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MonitorIsland.Helpers;

[SupportedOSPlatform("macos")]
internal sealed class MacOsSystemMetrics
{
    private const string LibSystem = "/usr/lib/libSystem.B.dylib";
    private const int HostCpuLoadInfoFlavor = 3;
    private const int HostVmInfoFlavor = 2;
    private const int Success = 0;

    private readonly object _cpuLock = new();

    private bool _hasCpuSample;
    private ulong _previousIdle;
    private ulong _previousTotal;

    public MacOsSystemMetrics()
    {
        _hasCpuSample = TryReadCpuTimes(
            out _previousIdle,
            out _previousTotal);
    }

    public double? GetCpuUsage()
    {
        lock (_cpuLock)
        {
            if (!TryReadCpuTimes(out ulong idle, out ulong total))
                return null;

            if (!_hasCpuSample || idle < _previousIdle || total < _previousTotal)
            {
                _previousIdle = idle;
                _previousTotal = total;
                _hasCpuSample = true;
                return null;
            }

            ulong idleDelta = idle - _previousIdle;
            ulong totalDelta = total - _previousTotal;
            _previousIdle = idle;
            _previousTotal = total;

            if (totalDelta == 0)
                return null;

            return Math.Clamp((totalDelta - Math.Min(idleDelta, totalDelta)) * 100d / totalDelta,
                              0d,
                              100d);
        }
    }

    public ulong? GetTotalMemory()
    {
        try
        {
            nuint length = sizeof(ulong);
            return SysctlByName(
                "hw.memsize",
                out ulong totalMemory,
                ref length,
                IntPtr.Zero,
                0) == Success
                ? totalMemory
                : null;
        }
        catch
        {
            return null;
        }
    }

    public double? GetAvailableMemory()
    {
        try
        {
            var statistics = new VmStatistics();
            uint count = (uint)(Marshal.SizeOf<VmStatistics>() / sizeof(int));
            if (GetVmStatistics(
                    MachHostSelf(),
                    HostVmInfoFlavor,
                    ref statistics,
                    ref count) != Success)
            {
                return null;
            }

            ulong availablePages =
                (ulong)statistics.FreeCount + statistics.InactiveCount;
            return availablePages * (ulong)Environment.SystemPageSize;
        }
        catch
        {
            return null;
        }
    }

    private static bool TryReadCpuTimes(out ulong idle, out ulong total)
    {
        idle = 0;
        total = 0;

        try
        {
            var cpuLoad = new HostCpuLoadInfo();
            uint count = (uint)(Marshal.SizeOf<HostCpuLoadInfo>() / sizeof(int));
            if (GetCpuStatistics(
                    MachHostSelf(),
                    HostCpuLoadInfoFlavor,
                    ref cpuLoad,
                    ref count) != Success)
            {
                return false;
            }

            idle = cpuLoad.Idle;
            total = (ulong)cpuLoad.User +
                cpuLoad.System +
                cpuLoad.Idle +
                cpuLoad.Nice;
            return true;
        }
        catch
        {
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HostCpuLoadInfo
    {
        public uint User;
        public uint System;
        public uint Idle;
        public uint Nice;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VmStatistics
    {
        public uint FreeCount;
        public uint ActiveCount;
        public uint InactiveCount;
        public uint WireCount;
        public uint ZeroFillCount;
        public uint Reactivations;
        public uint PageIns;
        public uint PageOuts;
        public uint Faults;
        public uint CopyOnWriteFaults;
        public uint Lookups;
        public uint Hits;
        public uint PurgeableCount;
        public uint Purges;
        public uint SpeculativeCount;
    }

    [DllImport(LibSystem, EntryPoint = "mach_host_self")]
    private static extern uint MachHostSelf();

    [DllImport(LibSystem, EntryPoint = "host_statistics")]
    private static extern int GetCpuStatistics(
        uint host,
        int flavor,
        ref HostCpuLoadInfo hostInfo,
        ref uint hostInfoCount);

    [DllImport(LibSystem, EntryPoint = "host_statistics")]
    private static extern int GetVmStatistics(
        uint host,
        int flavor,
        ref VmStatistics hostInfo,
        ref uint hostInfoCount);

    [DllImport(LibSystem, EntryPoint = "sysctlbyname")]
    private static extern int SysctlByName(
        [MarshalAs(UnmanagedType.LPStr)] string name,
        out ulong oldValue,
        ref nuint oldLength,
        IntPtr newValue,
        nuint newLength);
}
