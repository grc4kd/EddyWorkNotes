namespace Eddy.Requests;

public record TaskTimerRequest(TimeSpan WorkTimeSpan, TimeSpan BreakTimeSpan)
{
    public readonly TimeSpan TotalDuration = WorkTimeSpan + BreakTimeSpan;
}