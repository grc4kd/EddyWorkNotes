namespace ui.Components.State;

public readonly record struct TaskTimerStateValue
{
    public const string Stopped = "Stopped";
    public const string Running = "Running";
    public const string Paused = "Paused";
}
