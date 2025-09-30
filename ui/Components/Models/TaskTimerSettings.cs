namespace ui.Components.Models;

public class TaskTimerSettings(TimeSpan WorkTimeSpan, TimeSpan BreakTimeSpan)
{
    public TimeSpan WorkTimeSpan { get; } = WorkTimeSpan;
    public TimeSpan BreakTimeSpan { get; } = BreakTimeSpan;
}