namespace MonitorIsland.Models
{
    /// <summary>
    /// CPU�¶ȴ�������Ϣ
    /// </summary>
    public class CpuTemperatureSensorInfo
    {
        /// <summary>
        /// ������Ψһ��ʶ��
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ����������
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ӳ������
        /// </summary>
        public string HardwareName { get; set; } = string.Empty;

        /// <summary>
        /// ��ʾ�ı���Ӳ������ - ���������ƣ�
        /// </summary>
        public string DisplayText => $"{HardwareName} - {Name}";

        public override bool Equals(object? obj)
        {
            return obj is CpuTemperatureSensorInfo other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}