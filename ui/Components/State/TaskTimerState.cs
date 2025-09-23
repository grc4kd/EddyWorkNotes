namespace ui.Components.State;

public record TaskTimerState(string TimerStatus)
{
    public bool IsStopped => TimerStatus == TaskTimerStateValue.Stopped;
    public bool IsRunning => TimerStatus == TaskTimerStateValue.Running;
    public bool IsPaused => TimerStatus == TaskTimerStateValue.Paused;

    public static readonly TaskTimerState Stopped = new(TaskTimerStateValue.Stopped);
    public static readonly TaskTimerState Running = new(TaskTimerStateValue.Running);
    public static readonly TaskTimerState Paused = new(TaskTimerStateValue.Paused);

    public TaskTimerState(string status) => TimerStatus = status;

    private static void LoadValidStates()
    {
        ValidStates.Add("stop", "Stopped");
        ValidStates.Add("run", "Running");
        ValidStates.Add("pause", "Paused");
    }

    private static string PrintValidStates()
    {
        var sbStates = new StringBuilder();
        foreach (var state in ValidStates)
        {
            sbStates.Append(state + ", ");
        }
        return sbStates.ToString();
    }

    public bool IsStopped => timerStatus == ValidStates["stop"];
    public bool IsRunning => timerStatus == ValidStates["run"];
    public bool IsPaused => timerStatus == ValidStates["pause"];
}