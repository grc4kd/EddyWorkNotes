namespace Eddy.Requests;

public record TaskTimerRequest()
{
    public TimeSpan Duration { get; init; } = TimeSpan.FromTicks(1);

    public TaskTimerRequest(TimeSpan duration) : this()
    {
        // Validate timer requests against minimum/maximum supported TimeSpan
        ArgumentOutOfRangeException.ThrowIfLessThan(Duration, TimeSpan.FromTicks(1));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Duration, TimeSpan.MaxValue);

        Duration = duration;
    }
}