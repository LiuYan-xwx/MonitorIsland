using System.Runtime.InteropServices;

namespace MonitorIsland.Helpers
{
    public static class MemoryHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_INFO
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

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO memoryInfo);

        /// <summary>
        /// ��ȡϵͳ�������ڴ��С
        /// </summary>
        /// <returns>�������ڴ��С���ֽڣ�</returns>
        /// <exception cref="InvalidOperationException">�޷���ȡϵͳ�ڴ���Ϣʱ�׳�</exception>
        public static ulong GetTotalPhysicalMemory()
        {
            var memoryInfo = new MEMORY_INFO
            {
                Length = (uint)Marshal.SizeOf<MEMORY_INFO>()
            };
            GlobalMemoryStatusEx(ref memoryInfo);
            return memoryInfo.TotalPhysical;
        }
    }
}