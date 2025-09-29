namespace ui.Components.Models;

public record TaskTimerState(string TimerStatus)
{
    public bool IsStopped => TimerStatus == TaskTimerStateValue.Stopped;
    public bool IsRunning => TimerStatus == TaskTimerStateValue.Running;
    public bool IsPaused => TimerStatus == TaskTimerStateValue.Paused;

    public static readonly TaskTimerState Stopped = new(TaskTimerStateValue.Stopped);
    public static readonly TaskTimerState Running = new(TaskTimerStateValue.Running);
    public static readonly TaskTimerState Paused = new(TaskTimerStateValue.Paused);

    public override string ToString() => TimerStatus;
}