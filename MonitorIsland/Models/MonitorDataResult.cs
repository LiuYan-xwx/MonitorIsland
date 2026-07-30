namespace MonitorIsland.Models
{
    /// <summary>
    /// 表示监控数据读取结果，包含成功值、单位或错误信息。
    /// </summary>
    public readonly record struct MonitorDataResult
    {
        /// <summary>
        /// 获取一个值，指示当前结果是否成功。
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 获取或设置错误信息。
        /// </summary>
        /// <remarks>
        /// 该值可以为 <see langword="null"/>，表示失败但不提供具体错误描述。
        /// </remarks>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// 获取或设置监控数据的字符串值。
        /// </summary>
        public string? Value { get; init; }

        /// <summary>
        /// 获取或设置监控数据的显示单位。
        /// </summary>
        /// <remarks>
        /// 该值可为空，表示单位将沿用请求中所选择的单位。
        /// 如果请求中选择的单位为 <c>Auto</c>，则此项必须提供具体单位值。
        /// </remarks>
        public DisplayUnit? Unit { get; init; }

        /// <summary>
        /// 创建一个成功的监控数据结果。
        /// </summary>
        /// <param name="value">监控数据值。</param>
        /// <param name="unit">
        /// 显示单位。
        /// 可为空，表示使用请求中选择的单位；
        /// 但当请求选择的单位为 <c>Auto</c> 时，必须提供具体单位。
        /// </param>
        /// <returns>表示成功的 <see cref="MonitorDataResult"/> 实例。</returns>
        public static MonitorDataResult Success(string? value, DisplayUnit? unit = null)
            => new() { IsSuccess = true, Value = value, Unit = unit };

        /// <summary>
        /// 创建一个失败的监控数据结果。
        /// </summary>
        /// <param name="errorMessage">错误信息，可为空。</param>
        /// <returns>表示失败的 <see cref="MonitorDataResult"/> 实例。</returns>
        public static MonitorDataResult Error(string? errorMessage = null)
            => new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
