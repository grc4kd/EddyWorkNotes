namespace ui.Components.Models;

public readonly record struct TaskTimerStateValue
{
    private readonly TaskTimerState state = TaskTimerState.Stopped;

    public const string Stopped = "Stopped";
    public const string Running = "Running";

    public TaskTimerStateValue(TaskTimerState value)
    {
        state = value;
    }
}
