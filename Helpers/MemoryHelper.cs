using System.Runtime.InteropServices;

namespace MonitorIsland.Helpers
{
    public static class MemoryHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] ref MEMORYSTATUSEX lpBuffer);

        /// <summary>
        /// 获取系统总物理内存大小
        /// </summary>
        /// <returns>总物理内存大小（字节）</returns>
        /// <exception cref="InvalidOperationException">无法获取系统内存信息时抛出</exception>
        public static ulong GetTotalPhysicalMemory()
        {
            var memStatus = new MEMORYSTATUSEX
            {
                dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>()
            };

            if (GlobalMemoryStatusEx(ref memStatus))
            {
                return memStatus.ullTotalPhys;
            }

            return 0;
        }
    }
}