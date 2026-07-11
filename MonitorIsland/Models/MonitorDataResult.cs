namespace MonitorIsland.Models
{
    public readonly record struct MonitorDataResult
    {
        public bool IsSuccess => ErrorMessage is null;
        public string? ErrorMessage { get; init; }
        public string? Value { get; init; }
        public DisplayUnit? Unit { get; init; }

        public static MonitorDataResult Success(string? value, DisplayUnit? unit = null)
            => new() { Value = value, Unit = unit };

        public static MonitorDataResult Error(string? errorMessage)
            => new() { ErrorMessage = errorMessage };
    }
}
