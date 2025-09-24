namespace ui.Components.State;

public record TaskTimerState(string TimerStatus)
{
    public bool IsStopped => TimerStatus == TaskTimerStateValue.Stopped;
    public bool IsRunning => TimerStatus == TaskTimerStateValue.Running;
    public bool IsPaused => TimerStatus == TaskTimerStateValue.Paused;
}