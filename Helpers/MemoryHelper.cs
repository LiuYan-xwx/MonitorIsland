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
        /// ��ȡϵͳ�������ڴ��С
        /// </summary>
        /// <returns>�������ڴ��С���ֽڣ�</returns>
        /// <exception cref="InvalidOperationException">�޷���ȡϵͳ�ڴ���Ϣʱ�׳�</exception>
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