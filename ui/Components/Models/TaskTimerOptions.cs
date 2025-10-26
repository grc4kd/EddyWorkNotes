namespace ui.Components.Models;

public class TaskTimerOptions(TimeSpan WorkTimeSpan, TimeSpan BreakTimeSpan)
{
    public TaskTimerOptions() : this(TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5)) { }

    public TimeSpan WorkTimeSpan { get; } = WorkTimeSpan;
    public TimeSpan BreakTimeSpan { get; } = BreakTimeSpan;
}