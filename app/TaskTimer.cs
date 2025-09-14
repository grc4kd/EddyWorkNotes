namespace Eddy;


/// <summary>
/// 
/// </summary>
/// <param name="WorkDuration"></param>
/// <param name="BreakDuration"></param>
/// <param name="IsWorkTime"></param>
/// <param name="RemainingTime"></param>
public record TaskTimer(int WorkDuration, int BreakDuration, bool IsWorkTime)
{
    private readonly CancellationTokenSource _cts = new();

    public int WorkDuration { get; set; } = WorkDuration;
    public int BreakDuration { get; set; } = BreakDuration;
    public bool IsRunning { get; set; } = false;
    public bool IsWorkTime { get; set; } = IsWorkTime;
    public double RemainingTime { get; set; }

    public event EventHandler? TimeElapsed;
    public event EventHandler? TimerCompleted;

    private static PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1));

    /// <param name="IsWorkTime"></param>
    public TaskTimer(bool IsWorkTime, int WorkDuration, int BreakDuration) : this(WorkDuration, BreakDuration, IsWorkTime)
    {

    }
    public TaskTimer(int WorkDuration, int BreakDuration) : this(IsWorkTime: true, WorkDuration, BreakDuration)
    {

    }

    private void ResetTimer()
    {
        if (IsWorkTime)
        {
            TimeSpan initialTimeSpan = WorkDuration > 0 ?
                TimeSpan.FromSeconds(WorkDuration) :
                TimeSpan.FromMilliseconds(1);

            _timer = new(initialTimeSpan);
        }
        else
        {
            TimeSpan initialTimeSpan = BreakDuration > 0 ?
                TimeSpan.FromSeconds(BreakDuration) :
                TimeSpan.FromMilliseconds(1);

            _timer = new(initialTimeSpan);
        }

        RemainingTime = _timer.Period.TotalSeconds;
    }

    public void Cancel()
    {
        _cts.Cancel();
        OnTimerCompleted(EventArgs.Empty);
    }

    // write a test for OnTimeElapsed(EventArgs e) in test/TaskTimerTest.cs AI!
    public virtual void OnTimeElapsed(EventArgs e)
    {
        TimeElapsed?.Invoke(this, e);
    }

    public virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke(this, e);
    }

    public async Task StartAsync()
    {
        ResetTimer();

        if (_cts.IsCancellationRequested)
            TogglePause();

        await UpdateTimeAsync(_cts.Token);
    }

    public void TogglePause()
    {
        _cts.Cancel();
        IsRunning = !IsRunning;
    }

    private async Task UpdateTimeAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                TimerCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Stop and complete the timer after break is over
            IsRunning = IsWorkTime;

            await _timer.WaitForNextTickAsync(cancellationToken);

            // Switch to break time, then reset timer
            IsWorkTime = !IsWorkTime;

            // Raise the time elapsed event to subscribers
            TimeElapsed?.Invoke(this, EventArgs.Empty);

            ResetTimer();
        }
        catch (OperationCanceledException)
        {
            TimerCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
