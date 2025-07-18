using System.Runtime.InteropServices;

namespace MonitorIsland.Helpers
{
    public static class MemoryHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_INFO
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

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

        /// <summary>
        /// ��ȡϵͳ�������ڴ��С
        /// </summary>
        /// <returns>�������ڴ��С���ֽڣ�</returns>
        /// <exception cref="InvalidOperationException">�޷���ȡϵͳ�ڴ���Ϣʱ�׳�</exception>
        public static ulong GetTotalPhysicalMemory()
        {
            var memoryInfo = new MEMORY_INFO
            {
                dwLength = (uint)Marshal.SizeOf<MEMORY_INFO>()
            };
            GlobalMemoryStatusEx(ref memoryInfo);
            return memoryInfo.ullTotalPhys;
        }
    }
}