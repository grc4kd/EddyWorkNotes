namespace ui.Components.Models;

public class TaskTimerSettings(TimeSpan WorkTimeSpan, TimeSpan BreakTimeSpan)
{
    public TaskTimerSettings() : this(TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5)) { }

    public TimeSpan WorkTimeSpan { get; } = WorkTimeSpan;
    public TimeSpan BreakTimeSpan { get; } = BreakTimeSpan;
}