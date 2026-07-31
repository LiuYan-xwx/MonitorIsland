namespace MonitorIsland.Helpers;

internal sealed class SystemMetrics
{
    public static SystemMetrics Instance { get; } = new();

    private readonly WindowsSystemMetrics? _windowsMetrics;
    private readonly LinuxSystemMetrics? _linuxMetrics;
    private readonly MacOsSystemMetrics? _macOsMetrics;

    private SystemMetrics()
    {
        if (OperatingSystem.IsWindows())
            _windowsMetrics = new WindowsSystemMetrics();
        else if (OperatingSystem.IsLinux())
            _linuxMetrics = new LinuxSystemMetrics();
        else if (OperatingSystem.IsMacOS())
            _macOsMetrics = new MacOsSystemMetrics();
    }

    public double? GetCpuUsage()
    {
        if (OperatingSystem.IsWindows())
            return _windowsMetrics?.GetCpuUsage();

        if (OperatingSystem.IsLinux())
            return _linuxMetrics?.GetCpuUsage();

        if (OperatingSystem.IsMacOS())
            return _macOsMetrics?.GetCpuUsage();

        return null;
    }

    public ulong? GetTotalMemory()
    {
        if (OperatingSystem.IsWindows())
            return _windowsMetrics?.GetTotalMemory();

        if (OperatingSystem.IsLinux())
            return _linuxMetrics?.GetTotalMemory();

        if (OperatingSystem.IsMacOS())
            return _macOsMetrics?.GetTotalMemory();

        return null;
    }

    public double? GetAvailableMemory()
    {
        if (OperatingSystem.IsWindows())
            return _windowsMetrics?.GetAvailableMemory();

        if (OperatingSystem.IsLinux())
            return _linuxMetrics?.GetAvailableMemory();

        if (OperatingSystem.IsMacOS())
            return _macOsMetrics?.GetAvailableMemory();

        return null;
    }
}
