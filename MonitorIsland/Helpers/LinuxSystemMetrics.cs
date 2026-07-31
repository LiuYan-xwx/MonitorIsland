using System.Globalization;

namespace MonitorIsland.Helpers;

internal sealed class LinuxSystemMetrics
{
    private readonly object _cpuLock = new();

    private bool _hasCpuSample;
    private ulong _previousIdle;
    private ulong _previousTotal;

    public LinuxSystemMetrics()
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

            return Math.Clamp(
                (totalDelta - Math.Min(idleDelta, totalDelta)) * 100d / totalDelta,
                0d,
                100d);
        }
    }

    public ulong? GetTotalMemory()
    {
        return TryReadMemory(out ulong totalBytes, out _)
            ? totalBytes
            : null;
    }

    public double? GetAvailableMemory()
    {
        return TryReadMemory(out _, out ulong availableBytes)
            ? availableBytes
            : null;
    }

    private static bool TryReadCpuTimes(out ulong idle, out ulong total)
    {
        idle = 0;
        total = 0;

        try
        {
            string? cpuLine = File.ReadLines("/proc/stat").FirstOrDefault();
            if (cpuLine is null ||
                !cpuLine.StartsWith("cpu ", StringComparison.Ordinal))
            {
                return false;
            }

            string[] parts = cpuLine.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);
            if (parts.Length < 5)
                return false;

            for (int i = 1; i < parts.Length; i++)
            {
                if (!ulong.TryParse(
                        parts[i],
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out ulong value))
                {
                    return false;
                }

                total += value;
                if (i is 4 or 5)
                    idle += value;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryReadMemory(
        out ulong totalBytes,
        out ulong availableBytes)
    {
        totalBytes = 0;
        availableBytes = 0;

        try
        {
            ulong totalKb = 0;
            ulong availableKb = 0;
            ulong freeKb = 0;
            ulong buffersKb = 0;
            ulong cachedKb = 0;

            foreach (string line in File.ReadLines("/proc/meminfo"))
            {
                string[] parts = line.Split(
                    [' ', ':'],
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);
                if (parts.Length < 2 ||
                    !ulong.TryParse(
                        parts[1],
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out ulong value))
                {
                    continue;
                }

                switch (parts[0])
                {
                    case "MemTotal":
                        totalKb = value;
                        break;
                    case "MemAvailable":
                        availableKb = value;
                        break;
                    case "MemFree":
                        freeKb = value;
                        break;
                    case "Buffers":
                        buffersKb = value;
                        break;
                    case "Cached":
                        cachedKb = value;
                        break;
                }
            }

            if (totalKb == 0)
                return false;

            if (availableKb == 0)
                availableKb = freeKb + buffersKb + cachedKb;

            totalBytes = totalKb * 1024;
            availableBytes = availableKb * 1024;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
